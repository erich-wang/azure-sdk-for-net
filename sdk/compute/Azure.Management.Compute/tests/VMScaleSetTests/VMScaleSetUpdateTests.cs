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
using Sku = Azure.Management.Compute.Models.Sku;

namespace Azure.Management.Compute.Tests
{
    public class VMScaleSetUpdateTests : VMScaleSetTestsBase
    {
        public VMScaleSetUpdateTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// ScaleOut VMScaleSet
        /// ScaleIn VMScaleSet
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetScalingOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");


                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName, storageAccountOutput, imageRef);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                var getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                // Scale Out VMScaleSet
                inputVMScaleSet.Sku.Capacity = 3;
                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                // Scale In VMScaleSet
                inputVMScaleSet.Sku.Capacity = 1;
                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmScaleSet.Name);
            }
            finally
            {
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Update VMScaleSet
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetUpdateOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName, storageAccountOutput, imageRef);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                var getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                inputVMScaleSet.Sku.Name = VirtualMachineSizeTypes.StandardA1.ToString();
                VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
                {
                    Extensions = new List<VirtualMachineScaleSetExtension>()
                        {
                            GetTestVMSSVMExtension(),
                        }
                };
                inputVMScaleSet.VirtualMachineProfile.ExtensionProfile = extensionProfile;

                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmScaleSet.Name);
            }
            finally
            {
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        /// <summary>
        /// This is same as TestVMScaleSetUpdateOperations except that this test calls PATCH API instead of PUT
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Update VMScaleSet
        /// Scale VMScaleSet
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetPatchOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName, storageAccountOutput, imageRef);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                var getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                // Adding an extension to the VMScaleSet. We will use Patch to update this.
                VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
                {
                    Extensions = new List<VirtualMachineScaleSetExtension>()
                        {
                            GetTestVMSSVMExtension(),
                        }
                };

                VirtualMachineScaleSetUpdate patchVMScaleSet = new VirtualMachineScaleSetUpdate()
                {
                    VirtualMachineProfile = new VirtualMachineScaleSetUpdateVMProfile()
                    {
                        ExtensionProfile = extensionProfile,
                    },
                };
                PatchVMScaleSet(rgName, vmssName, patchVMScaleSet);

                // Update the inputVMScaleSet and then compare it with response to verify the result.
                inputVMScaleSet.VirtualMachineProfile.ExtensionProfile = extensionProfile;
                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);


                // Scaling the VMScaleSet now to 3 instances
                VirtualMachineScaleSetUpdate patchVMScaleSet2 = new VirtualMachineScaleSetUpdate()
                {
                    Sku = new Sku()
                    {
                        Capacity = 3,
                    },
                };
                PatchVMScaleSet(rgName, vmssName, patchVMScaleSet2);

                // Validate that ScaleSet Scaled to 3 instances
                inputVMScaleSet.Sku.Capacity = 3;
                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmScaleSet.Name);
                ValidateVMScaleSet(inputVMScaleSet, getResponse);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmScaleSet.Name);
            }
            finally
            {
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }
    }
}
