﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using System.Reflection;
using System.Threading;
using Azure.Management.Compute.Tests;
using Azure.Management.Compute;
using Azure.Management.Compute.Models;
using System.Text.Json;
using System.Net;

namespace Azure.Management.Compute.Tests
{
    public class VMScaleSetExtensionTests : VMScaleSetTestsBase
    {
        public VMScaleSetExtensionTests(bool isAsync)
       : base(isAsync)
        {
        }
        //[SetUp]
        //public void ClearChallengeCacheforRecord()
        //{
        //    if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
        //    {
        //        InitializeBase();
        //    }
        //    //ComputeManagementClient computeClient;
        //    //ResourceManagementClient resourcesClient;
        //}

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Create VMSS Extension
        /// Update VMSS Extension
        /// Get VMSS Extension
        /// List VMSS Extensions
        /// Delete VMSS Extension
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetExtensions()
        {
            await TestVMScaleSetExtensionsImpl();
        }

        [Test]
        public async Task TestVMScaleSetExtensionSequencing()
        {
            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            VirtualMachineScaleSet inputVMScaleSet;
            try
            {
                EnsureClientsInitialized();
                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: false);
                VirtualMachineScaleSetExtensionProfile vmssExtProfile = GetTestVmssExtensionProfile();

                // Set extension sequencing (ext2 is provisioned after ext1)
                vmssExtProfile.Extensions[1].ProvisionAfterExtensions = new List<string> { vmssExtProfile.Extensions[0].Name };

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    null,
                    imageRef,
                    extensionProfile: vmssExtProfile,
                    createWithManagedDisks: true);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                // Perform a Get operation on each extension
                VirtualMachineScaleSetExtension getVmssExtResponse = null;
                for (int i = 0; i < vmssExtProfile.Extensions.Count; i++)
                {
                    getVmssExtResponse = await VirtualMachineScaleSetExtensionsClient.GetAsync(rgName, vmssName, vmssExtProfile.Extensions[i].Name);
                    ValidateVmssExtension(vmssExtProfile.Extensions[i], getVmssExtResponse);
                }

                // Add a new extension to the VMSS (ext3 is provisioned after ext2)
                VirtualMachineScaleSetExtension vmssExtension = GetTestVMSSVMExtension(name: "3", publisher: "Microsoft.CPlat.Core", type: "NullLinux", version: "4.0");
                vmssExtension.ProvisionAfterExtensions = new List<string> { vmssExtProfile.Extensions[1].Name };
                var response = await VirtualMachineScaleSetExtensionsClient.StartCreateOrUpdateAsync(rgName, vmssName, vmssExtension.Name, vmssExtension);
                ValidateVmssExtension(vmssExtension, response.Value);

                // Perform a Get operation on the extension
                getVmssExtResponse = await VirtualMachineScaleSetExtensionsClient.GetAsync(rgName, vmssName, vmssExtension.Name);
                ValidateVmssExtension(vmssExtension, getVmssExtResponse);

                // Clear the sequencing in ext3
                vmssExtension.ProvisionAfterExtensions.Clear();
                var patchVmssExtsResponse = await VirtualMachineScaleSetExtensionsClient.StartCreateOrUpdateAsync(rgName, vmssName, vmssExtension.Name, vmssExtension);
                ValidateVmssExtension(vmssExtension, patchVmssExtsResponse.Value);

                // Perform a List operation on vmss extensions
                var listVmssExts =  VirtualMachineScaleSetExtensionsClient.ListAsync(rgName, vmssName);
                var listVmssExtsResponse = await listVmssExts.ToEnumerableAsync();
                int installedExtensionsCount = listVmssExtsResponse.Count();
                Assert.AreEqual(3, installedExtensionsCount);
                VirtualMachineScaleSetExtension expectedVmssExt = null;
                for (int i = 0; i < installedExtensionsCount; i++)
                {
                    if (i < installedExtensionsCount - 1)
                    {
                        expectedVmssExt = vmssExtProfile.Extensions[i];
                    }
                    else
                    {
                        expectedVmssExt = vmssExtension;
                    }

                    ValidateVmssExtension(expectedVmssExt, listVmssExtsResponse.ElementAt(i));
                }
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        private VirtualMachineScaleSetExtensionProfile GetTestVmssExtensionProfile()
        {
            return new VirtualMachineScaleSetExtensionProfile
            {
                Extensions = new List<VirtualMachineScaleSetExtension>()
                {
                    GetTestVMSSVMExtension(name: "1", publisher: "Microsoft.CPlat.Core", type: "NullSeqA", version: "2.0"),
                    GetTestVMSSVMExtension(name: "2", publisher: "Microsoft.CPlat.Core", type: "NullSeqB", version: "2.0")
                }
            };
        }

        private async Task TestVMScaleSetExtensionsImpl()
        {
            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            bool passed = false;
            try
            {
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef);

                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                //VirtualMachineScaleSet vmScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                //    rgName,
                //    vmssName,
                //    storageAccountOutput,
                //    imageRef);

                // Add an extension to the VMSS
                VirtualMachineScaleSetExtension vmssExtension = GetTestVMSSVMExtension();
                vmssExtension.ForceUpdateTag = "RerunExtension";
                var response = await VirtualMachineScaleSetExtensionsClient.StartCreateOrUpdateAsync(rgName, vmssName, vmssExtension.Name, vmssExtension);
                ValidateVmssExtension(vmssExtension, response.Value);

                // Perform a Get operation on the extension
                var getVmssExtResponse = await VirtualMachineScaleSetExtensionsClient.GetAsync(rgName, vmssName, vmssExtension.Name);
                ValidateVmssExtension(vmssExtension, getVmssExtResponse);

                // Validate the extension instance view in the VMSS instance-view
                var getVmssWithInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                ValidateVmssExtensionInstanceView(getVmssWithInstanceViewResponse.Value.Extensions.FirstOrDefault());

                // Update an extension in the VMSS
                vmssExtension.Settings = string.Empty;
                var patchVmssExtsResponse = await VirtualMachineScaleSetExtensionsClient.StartCreateOrUpdateAsync(rgName, vmssName, vmssExtension.Name, vmssExtension);
                ValidateVmssExtension(vmssExtension, patchVmssExtsResponse.Value);

                // Perform a List operation on vmss extensions
                var listVmssExts = VirtualMachineScaleSetExtensionsClient.ListAsync(rgName, vmssName);
                var listVmssExtsResponse = await listVmssExts.ToEnumerableAsync();
                ValidateVmssExtension(vmssExtension, listVmssExtsResponse.FirstOrDefault(c => c.ForceUpdateTag == "RerunExtension"));

                // Validate the extension delete API
                await VirtualMachineScaleSetExtensionsClient.StartDeleteAsync(rgName, vmssName, vmssExtension.Name);

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }

            Assert.True(passed);
        }

        protected void ValidateVmssExtension(VirtualMachineScaleSetExtension vmssExtension, VirtualMachineScaleSetExtension vmssExtensionOut)
        {
            Assert.NotNull(vmssExtensionOut);
            Assert.True(!string.IsNullOrEmpty(vmssExtensionOut.ProvisioningState));

            Assert.True(vmssExtension.Publisher == vmssExtensionOut.Publisher);
            Assert.True(vmssExtension.Type == vmssExtensionOut.Type);
            Assert.True(vmssExtension.AutoUpgradeMinorVersion == vmssExtensionOut.AutoUpgradeMinorVersion);
            Assert.True(vmssExtension.TypeHandlerVersion == vmssExtensionOut.TypeHandlerVersion);
            Assert.True(vmssExtension.Settings.ToString() == vmssExtensionOut.Settings.ToString());
            Assert.True(vmssExtension.ForceUpdateTag == vmssExtensionOut.ForceUpdateTag);

            if (vmssExtension.ProvisionAfterExtensions != null)
            {
                Assert.True(vmssExtension.ProvisionAfterExtensions.Count == vmssExtensionOut.ProvisionAfterExtensions.Count);
                for (int i = 0; i < vmssExtension.ProvisionAfterExtensions.Count; i++)
                {
                    Assert.True(vmssExtension.ProvisionAfterExtensions[i] == vmssExtensionOut.ProvisionAfterExtensions[i]);
                }
            }
        }

        protected void ValidateVmssExtensionInstanceView(VirtualMachineScaleSetVMExtensionsSummary vmssExtSummary)
        {
            Assert.NotNull(vmssExtSummary);
            Assert.NotNull(vmssExtSummary.Name);
            Assert.NotNull(vmssExtSummary.StatusesSummary[0].Code);
            Assert.NotNull(vmssExtSummary.StatusesSummary[0].Count);
        }
    }
}
