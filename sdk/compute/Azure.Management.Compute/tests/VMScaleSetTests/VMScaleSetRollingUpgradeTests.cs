// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;


namespace Azure.Management.Compute.Tests
{
    public class VMScaleSetRollingUpgradeTests : VMScaleSetTestsBase
    {
        public VMScaleSetRollingUpgradeTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources with an SLB probe to use as a health probe
        /// Create VMScaleSet in rolling upgrade mode
        /// Get VMScaleSet Model View
        /// Get VMScaleSet Instance View
        /// Upgrade scale set with an extension
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetRollingUpgrade")]
        public async Task TestVMScaleSetRollingUpgrade()
        {
                string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

                // Create resource group
                var rgName = Recording.GenerateAssetName(TestPrefix);
                var vmssName = Recording.GenerateAssetName("vmss");
                string storageAccountName = Recording.GenerateAssetName(TestPrefix);
                VirtualMachineScaleSet inputVMScaleSet;
                try
                {
                    Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "southcentralus");
                    EnsureClientsInitialized();
                    ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

                    VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
                    {
                        Extensions = new List<VirtualMachineScaleSetExtension>()
                        {
                            GetTestVMSSVMExtension(),
                        }
                    };

                    var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                    await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                    var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                        rgName,
                        vmssName,
                        storageAccountOutput,
                        imageRef,
                        null,
                        (vmScaleSet) =>
                        {
                            vmScaleSet.Overprovision = false;
                            vmScaleSet.UpgradePolicy.Mode = UpgradeMode.Rolling;
                        },
                        createWithManagedDisks: true,
                        createWithPublicIpAddress: false,
                        createWithHealthProbe: true);
                    VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                    inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                    var getInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                    Assert.NotNull(getInstanceViewResponse);
                    ValidateVMScaleSetInstanceView(inputVMScaleSet, getInstanceViewResponse);

                    var getVMInstanceViewResponse = await VirtualMachineScaleSetVMsClient.GetInstanceViewAsync(rgName, vmssName, "0");
                    Assert.NotNull(getVMInstanceViewResponse);
                    Assert.NotNull(getVMInstanceViewResponse.Value.VmHealth);
                    Assert.AreEqual("HealthState/healthy", getVMInstanceViewResponse.Value.VmHealth.Status.Code);

                    // Update the VMSS by adding an extension
                    WaitSeconds(600);
                    var vmssStatus = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);

                    inputVMScaleSet.VirtualMachineProfile.ExtensionProfile = extensionProfile;
                    UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                    getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                    getInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                    Assert.NotNull(getInstanceViewResponse);
                    ValidateVMScaleSetInstanceView(inputVMScaleSet, getInstanceViewResponse);

                    await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmssName));
                }
                finally
                {
                    Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                    //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                    //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                    await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
                }
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources with an SLB probe to use as a health probe
        /// Create VMScaleSet in rolling upgrade mode
        /// Perform a rolling OS upgrade
        /// Validate the rolling upgrade completed
        /// Perform another rolling OS upgrade
        /// Cancel the rolling upgrade
        /// Delete RG
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetRollingUpgradeAPIs")]
        public async Task TestVMScaleSetRollingUpgradeAPIs()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "southcentralus");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                imageRef.Version = "latest";

                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    null,
                    (vmScaleSet) =>
                    {
                        vmScaleSet.Overprovision = false;
                        vmScaleSet.UpgradePolicy.Mode = UpgradeMode.Rolling;
                        vmScaleSet.UpgradePolicy.AutomaticOSUpgradePolicy = new AutomaticOSUpgradePolicy()
                        {
                            EnableAutomaticOSUpgrade = false
                        };
                    },
                    createWithManagedDisks: true,
                    createWithPublicIpAddress: false,
                    createWithHealthProbe: true);
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
                WaitSeconds(600);
                var vmssStatus = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetRollingUpgradesClient.StartStartOSUpgradeAsync(rgName, vmssName));
                var rollingUpgradeStatus = await VirtualMachineScaleSetRollingUpgradesClient.GetLatestAsync(rgName, vmssName);
                Assert.AreEqual(inputVMScaleSet.Sku.Capacity, rollingUpgradeStatus.Value.Progress.SuccessfulInstanceCount);

                var upgradeTask = await WaitForCompletionAsync(await VirtualMachineScaleSetRollingUpgradesClient.StartStartOSUpgradeAsync(rgName, vmssName));
                vmssStatus = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetRollingUpgradesClient.StartCancelAsync(rgName, vmssName));

                rollingUpgradeStatus = await VirtualMachineScaleSetRollingUpgradesClient.GetLatestAsync(rgName, vmssName);

                Assert.True(rollingUpgradeStatus.Value.RunningStatus.Code == RollingUpgradeStatusCode.Cancelled);
                Assert.True(rollingUpgradeStatus.Value.Progress.PendingInstanceCount >= 0);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources with an SLB probe to use as a health probe
        /// Create VMScaleSet in rolling upgrade mode
        /// Perform a rolling OS upgrade
        /// Validate Upgrade History
        /// Delete RG
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetRollingUpgradeHistory")]
        public async Task TestVMScaleSetRollingUpgradeHistory()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "southcentralus");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                imageRef.Version = "latest";

                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    null,
                    (vmScaleSet) =>
                    {
                        vmScaleSet.Overprovision = false;
                        vmScaleSet.UpgradePolicy.Mode = UpgradeMode.Rolling;
                    },
                    createWithManagedDisks: true,
                    createWithPublicIpAddress: false,
                    createWithHealthProbe: true);
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
                WaitSeconds(600);
                var vmssStatus = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetRollingUpgradesClient.StartStartOSUpgradeAsync(rgName, vmssName));
                var rollingUpgrade = VirtualMachineScaleSetsClient.GetOSUpgradeHistoryAsync(rgName, vmssName);
                var rollingUpgradeHistory = await rollingUpgrade.ToEnumerableAsync();
                Assert.NotNull(rollingUpgradeHistory);
                Assert.True(rollingUpgradeHistory.Count() == 1);
                Assert.AreEqual(inputVMScaleSet.Sku.Capacity, rollingUpgradeHistory.First().Properties.Progress.SuccessfulInstanceCount);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }

        /// <summary>
        /// Testing Automatic OS Upgrade Policy
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetAutomaticOSUpgradePolicies")]
        public async Task TestVMScaleSetAutomaticOSUpgradePolicies()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "westcentralus");
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            imageRef.Version = "latest";

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    null,
                    (vmScaleSet) =>
                    {
                        vmScaleSet.Overprovision = false;
                        vmScaleSet.UpgradePolicy.AutomaticOSUpgradePolicy = new AutomaticOSUpgradePolicy()
                        {
                            DisableAutomaticRollback = false
                        };
                    },
                    createWithManagedDisks: true,
                    createWithPublicIpAddress: false,
                    createWithHealthProbe: true);
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                // Set Automatic OS Upgrade
                inputVMScaleSet.UpgradePolicy.AutomaticOSUpgradePolicy.EnableAutomaticOSUpgrade = true;
                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                // with automatic OS upgrade policy as null
                inputVMScaleSet.UpgradePolicy.AutomaticOSUpgradePolicy = null;
                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
                Assert.NotNull(getResponse.UpgradePolicy.AutomaticOSUpgradePolicy);
                Assert.True(getResponse.UpgradePolicy.AutomaticOSUpgradePolicy.DisableAutomaticRollback == false);
                Assert.True(getResponse.UpgradePolicy.AutomaticOSUpgradePolicy.EnableAutomaticOSUpgrade == true);

                // Toggle Disable Auto Rollback
                inputVMScaleSet.UpgradePolicy.AutomaticOSUpgradePolicy = new AutomaticOSUpgradePolicy()
                {
                    DisableAutomaticRollback = true,
                    EnableAutomaticOSUpgrade = false
                };
                UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }

        // Does the following operations:
        // Create ResourceGroup
        // Create StorageAccount
        // Create VMSS in Automatic Mode
        // Perform an extension rolling upgrade
        // Delete ResourceGroup
        [Test]
        //[Ignore ""]
        //[Trait("Name", "TestVMScaleSetExtensionUpgradeAPIs")]
        public async Task TestVMScaleSetExtensionUpgradeAPIs()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string rgName = Recording.GenerateAssetName(TestPrefix);
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                EnsureClientsInitialized();

                // Windows VM image
                ImageReference imageRef = await GetPlatformVMImage(true);
                imageRef.Version = "latest";
                var extension = GetTestVMSSVMExtension();
                VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
                {
                    Extensions = new List<VirtualMachineScaleSetExtension>()
                {
                    extension,
                }
                };

                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist"));

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    extensionProfile,
                    (vmScaleSet) =>
                    {
                        vmScaleSet.Overprovision = false;
                        vmScaleSet.UpgradePolicy.Mode = UpgradeMode.Automatic;
                    },
                    createWithManagedDisks: true,
                    createWithPublicIpAddress: false,
                    createWithHealthProbe: true);
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                await WaitForCompletionAsync(await VirtualMachineScaleSetRollingUpgradesClient.StartStartExtensionUpgradeAsync(rgName, vmssName));
                var rollingUpgradeStatus = await VirtualMachineScaleSetRollingUpgradesClient.GetLatestAsync(rgName, vmssName);
                Assert.AreEqual(inputVMScaleSet.Sku.Capacity, rollingUpgradeStatus.Value.Progress.SuccessfulInstanceCount);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup resource group and revert default location to the original location
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
    }
}
