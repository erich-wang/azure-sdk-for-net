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
using Castle.Core.Logging;

namespace Azure.Management.Compute.Tests
{
    public class ExtensionTests:VMTestBase
    {
        public ExtensionTests(bool isAsync)
           : base(isAsync)
        {
        }
        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                InitializeBase();
            }
            //ComputeManagementClient computeClient;
            //ResourceManagementClient resourcesClient;
        }

        private VirtualMachineExtension GetTestVMExtension()
        {
            var vmExtension = new VirtualMachineExtension(null, "vmext01", "Microsoft.Compute/virtualMachines/extensions", "SoutheastAsia",
                new Dictionary<string, string>() { { "extensionTag1", "1" }, { "extensionTag2", "2" } },
                "RerunExtension",
                "Microsoft.Compute",
                 "VMAccessAgent", "2.0", true, "{}", "{}", null, null
                );
            //{
            //    Tags = new Dictionary<string, string>() { { "extensionTag1", "1" }, { "extensionTag2", "2" } },
            //    Publisher = "Microsoft.Compute",
            //    //VirtualMachineExtensionType = "VMAccessAgent",
            //    TypeHandlerVersion = "2.0",
            //    AutoUpgradeMinorVersion = true,
            //    ForceUpdateTag = "RerunExtension",
            //    Settings = "{}",
            //    ProtectedSettings = "{}"
            //};
            //typeof(Compute.Models.Resource).GetRuntimeProperty("Name").SetValue(vmExtension, "vmext01");
            //typeof(Compute.Models.Resource).GetRuntimeProperty("Type").SetValue(vmExtension, "Microsoft.Compute/virtualMachines/extensions");

            return vmExtension;
        }

        private VirtualMachineExtensionUpdate GetTestVMUpdateExtension()
        {
            var vmExtensionUpdate = new VirtualMachineExtensionUpdate
            {
                Tags =
                    new Dictionary<string, string>
                    {
                        {"extensionTag1", "1"},
                        {"extensionTag2", "2"},
                        {"extensionTag3", "3"}
                    }
            };

            return vmExtensionUpdate;
        }

        [Test]
        public async Task TestVMExtensionOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            VirtualMachine inputVM;
            //try
            //{
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var returnTwovm = await CreateVM(rgName, asName, storageAccountOutput, imageRef);
                var vm = returnTwovm.Item1;
                inputVM = returnTwovm.Item2;
                // Delete an extension that does not exist in the VM. A http status code of NoContent should be returned which translates to operation success.
                var xx =await (await VirtualMachineExtensionsClient.StartDeleteAsync(rgName, vm.Name, "VMExtensionDoesNotExist")).WaitForCompletionAsync();
                // Add an extension to the VM
                var vmExtension = GetTestVMExtension();
                var resp = await VirtualMachineExtensionsClient.StartCreateOrUpdateAsync(rgName, vm.Name, vmExtension.Name, vmExtension);
                var response =await resp.WaitForCompletionAsync();
                ValidateVMExtension(vmExtension, response);

                // Perform a Get operation on the extension
                var getVMExtResponse = await VirtualMachineExtensionsClient.GetAsync(rgName, vm.Name, vmExtension.Name);
                ValidateVMExtension(vmExtension, getVMExtResponse);

                // Perform a GetExtensions on the VM
                var getVMExtsResponse = (await VirtualMachineExtensionsClient.ListAsync(rgName, vm.Name)).Value;

                Assert.True(getVMExtsResponse.Value.Count > 0);
                var vme = getVMExtsResponse.Value.Where(c => c.Name == "vmext01");
                Assert.AreEqual(vme.Count(),1);
                //Assert.Single(vme);
                ValidateVMExtension(vmExtension, vme.First());

                // Validate Get InstanceView for the extension
                var getVMExtInstanceViewResponse = (await VirtualMachineExtensionsClient.GetAsync(rgName, vm.Name, vmExtension.Name, "instanceView")).Value;
                ValidateVMExtensionInstanceView(getVMExtInstanceViewResponse.InstanceView);

                // Update extension on the VM
                var vmExtensionUpdate = GetTestVMUpdateExtension();
                await (await VirtualMachineExtensionsClient.StartUpdateAsync(rgName, vm.Name, vmExtension.Name, vmExtensionUpdate)).WaitForCompletionAsync();
                vmExtension.Tags["extensionTag3"] = "3";
                getVMExtResponse = await VirtualMachineExtensionsClient.GetAsync(rgName, vm.Name, vmExtension.Name);
                ValidateVMExtension(vmExtension, getVMExtResponse);

                // Validate the extension in the VM info
                var getVMResponse = (await VirtualMachinesClient.GetAsync(rgName, vm.Name)).Value;
                // TODO AutoRest: Recording Passed, but these assertions failed in Playback mode
                ValidateVMExtension(vmExtension, getVMResponse.Resources.FirstOrDefault(c => c.Name == vmExtension.Name));

                // Validate the extension instance view in the VM instance-view
                var getVMWithInstanceViewResponse = (await VirtualMachinesClient.GetAsync(rgName, vm.Name)).Value;
                ValidateVMExtensionInstanceView(getVMWithInstanceViewResponse.InstanceView.Extensions.FirstOrDefault());

                // Validate the extension delete API
                await (await VirtualMachineExtensionsClient.StartDeleteAsync(rgName, vm.Name, vmExtension.Name)).WaitForCompletionAsync();
            //}
            //finally
            //{
            //    //await ResourceGroupsClient.StartDeleteAsync(rgName);
            //}
        }

        private void ValidateVMExtension(VirtualMachineExtension vmExtExpected, VirtualMachineExtension vmExtReturned)
        {
            Assert.NotNull(vmExtReturned);
            Assert.True(!string.IsNullOrEmpty(vmExtReturned.ProvisioningState));

            Assert.True(vmExtExpected.Publisher == vmExtReturned.Publisher);
            Assert.True(vmExtExpected.TypePropertiesType == vmExtReturned.TypePropertiesType);
            Assert.True(vmExtExpected.AutoUpgradeMinorVersion == vmExtReturned.AutoUpgradeMinorVersion);
            Assert.True(vmExtExpected.TypeHandlerVersion == vmExtReturned.TypeHandlerVersion);
            Assert.True(vmExtExpected.Settings.ToString() == vmExtReturned.Settings.ToString());
            Assert.True(vmExtExpected.ForceUpdateTag == vmExtReturned.ForceUpdateTag);
            Assert.True(vmExtExpected.Tags.SequenceEqual(vmExtReturned.Tags));
        }

        private void ValidateVMExtensionInstanceView(VirtualMachineExtensionInstanceView vmExtInstanceView)
        {
            Assert.NotNull(vmExtInstanceView);
            Assert.NotNull(vmExtInstanceView.Statuses[0].DisplayStatus);
            Assert.NotNull(vmExtInstanceView.Statuses[0].Code);
            Assert.NotNull(vmExtInstanceView.Statuses[0].Level);
            Assert.NotNull(vmExtInstanceView.Statuses[0].Message);
        }
    }
}
