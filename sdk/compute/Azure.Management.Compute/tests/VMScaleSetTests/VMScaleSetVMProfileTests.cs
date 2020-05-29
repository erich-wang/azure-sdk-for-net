// Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Azure.Management.Compute.Tests.VMScaleSetTests
{
    public class VMScaleSetVMProfileTests : VMScaleSetTestsBase
    {
        public VMScaleSetVMProfileTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Checks if licenseType can be set through API
        /// </summary>
        [Test]
        public async Task TestVMScaleSetWithLicenseType()
        {
            EnsureClientsInitialized();

            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

            VirtualMachineScaleSet inputVMScaleSet;

            Action<VirtualMachineScaleSet> vmProfileCustomizer = vmss =>
            {
                vmss.VirtualMachineProfile.StorageProfile.ImageReference = GetPlatformVMImage(true).Result;
                vmss.VirtualMachineProfile.LicenseType = "Windows_Server";
            };

            try
            {
                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName: rgName,
                    vmssName: vmssName,
                    storageAccount: storageAccountOutput,
                    imageRef: null,
                    createWithManagedDisks: true,
                    vmScaleSetCustomizer: vmProfileCustomizer
                    );
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;

                var response = (await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName)).Value;

                Assert.AreEqual("Windows_Server", response.VirtualMachineProfile.LicenseType);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("One or more errors occurred while preparing VM disks. See disk instance view for details."))
                {
                    return;
                }
                throw;
            }
            finally
            {
                //await ResourceGroupsClient.DeleteIfExists(rgName);
                await (await ResourceGroupsClient.StartDeleteAsync(rgName)).WaitForCompletionAsync();
            }
        }


        /// <summary>
        /// Checks if diagnostics profile can be set through API
        /// </summary>
        [Test]
        public async Task TestVMScaleSetDiagnosticsProfile()
        {
            EnsureClientsInitialized();

            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

            VirtualMachineScaleSet inputVMScaleSet;

            Action<VirtualMachineScaleSet> vmProfileCustomizer = vmss =>
            {
                vmss.VirtualMachineProfile.DiagnosticsProfile = new DiagnosticsProfile(
                    new BootDiagnostics
                    {
                        Enabled = true,
                        StorageUri = string.Format(Constants.StorageAccountBlobUriTemplate, storageAccountName)
                    });
            };

            try
            {
                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName: rgName,
                    vmssName: vmssName,
                    storageAccount: storageAccountOutput,
                    imageRef: await GetPlatformVMImage(true),
                    createWithManagedDisks: true,
                    vmScaleSetCustomizer: vmProfileCustomizer
                    );
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                var response = (await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName)).Value;

                Assert.True(response.VirtualMachineProfile.DiagnosticsProfile.BootDiagnostics.Enabled.GetValueOrDefault(true));
            }
            finally
            {
                //await ResourceGroupsClient.DeleteIfExists(rgName);
                await (await ResourceGroupsClient.StartDeleteAsync(rgName)).WaitForCompletionAsync();
            }
        }
    }
}
