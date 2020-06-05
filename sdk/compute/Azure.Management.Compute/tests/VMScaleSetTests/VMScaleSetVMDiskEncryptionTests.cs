// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class VMScaleSetVMDiskEncryptionTests : VMScaleSetVMTestsBase
    {
        public VMScaleSetVMDiskEncryptionTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create VMScaleSet with extension
        /// Get VMScaleSet VM instance view
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        [Ignore ("This test should be skipped")]
        //[Test(Skip = "ReRecord due to CR change")]
        //[Trait("Name", "TestVMScaleSetVMDiskEncryptionOperation")]
        public async Task TestVMScaleSetVMDiskEncryptionOperation()
        {
            await TestDiskEncryptionOnScaleSetVMInternal(hasManagedDisks: true, useVmssExtension: true);
        }

        private async Task TestDiskEncryptionOnScaleSetVMInternal(bool hasManagedDisks = true, bool useVmssExtension = true)
        {
            EnsureClientsInitialized();

            // Get platform image for VMScaleSet create
            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix);
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            var dnsname = Recording.GenerateAssetName("dnsname");

            // Create ADE extension to enable disk encryption
            VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
            {
                Extensions = new List<VirtualMachineScaleSetExtension>()
                {
                    GetAzureDiskEncryptionExtension(),
                }
            };

            bool testSucceeded = false;
            try
            {
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await WaitForCompletionAsync( await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                var vnetResponse = await CreateVNETWithSubnets(rgName, 2);
                var vmssSubnet = vnetResponse.Subnets[1];

                var publicipConfiguration = new VirtualMachineScaleSetPublicIPAddressConfiguration("pip1");
                publicipConfiguration.IdleTimeoutInMinutes = 10;
                publicipConfiguration.DnsSettings = new VirtualMachineScaleSetPublicIPAddressConfigurationDnsSettings(dnsname);

                VirtualMachineScaleSet inputVMScaleSet;
                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    useVmssExtension ? extensionProfile : null,
                    (vmss) =>
                    {
                        vmss.Sku.Name = VirtualMachineSizeTypes.StandardA3.ToString();
                        vmss.Sku.Tier = "Standard";
                        vmss.VirtualMachineProfile.StorageProfile.OsDisk = new VirtualMachineScaleSetOSDisk(DiskCreateOptionTypes.FromImage);
                        vmss.VirtualMachineProfile.NetworkProfile
                                    .NetworkInterfaceConfigurations[0].IpConfigurations[0].PublicIPAddressConfiguration = publicipConfiguration;
                    },
                    createWithManagedDisks: hasManagedDisks,
                    createWithPublicIpAddress: false,
                    subnet: vmssSubnet);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;

                VirtualMachineScaleSetVMInstanceView vmInstanceViewResponse =
                    await VirtualMachineScaleSetVMsClient.GetInstanceViewAsync(rgName, vmScaleSet.Name, "0");
                Assert.True(vmInstanceViewResponse != null, "VMScaleSetVM not returned.");

                ValidateEncryptionSettingsInVMScaleSetVMInstanceView(vmInstanceViewResponse, hasManagedDisks);

                await WaitForCompletionAsync( await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmssName));
                testSucceeded = true;
            }
            finally
            {
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(testSucceeded);
        }
    }
}
