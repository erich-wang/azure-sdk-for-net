// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class VMScaleSetOperationalTests : VMScaleSetTestsBase
    {
        public VMScaleSetOperationalTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Start VMScaleSet
        /// Stop VMScaleSet
        /// Restart VMScaleSet
        /// Deallocate VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetOperations()
        {
            await TestVMScaleSetOperationsInternal();
        }

        /// <summary>
        /// Covers following Operations for a ScaleSet with ManagedDisks:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Start VMScaleSet
        /// Reimage VMScaleSet
        /// ReimageAll VMScaleSet
        /// Stop VMScaleSet
        /// Restart VMScaleSet
        /// Deallocate VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetOperations_ManagedDisks()
        {
            await TestVMScaleSetOperationsInternal(hasManagedDisks: true);
        }

        private async Task TestVMScaleSetOperationsInternal(bool hasManagedDisks = false)
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            bool passed = false;
            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    createWithManagedDisks: hasManagedDisks);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                // TODO: AutoRest skips the following methods - Start, Restart, PowerOff, Deallocate
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartStartAsync(rgName, vmScaleSet.Name));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartReimageAsync(rgName, vmScaleSet.Name));
                if (hasManagedDisks)
                {
                    await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartReimageAllAsync(rgName, vmScaleSet.Name));
                }
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartRestartAsync(rgName, vmScaleSet.Name));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartPowerOffAsync(rgName, vmScaleSet.Name));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeallocateAsync(rgName, vmScaleSet.Name));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmScaleSet.Name));

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        [Test]
        public async Task TestVMScaleSetOperations_Redeploy()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            bool passed = false;

            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "EastUS2");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName,
                    storageAccountOutput, imageRef, createWithManagedDisks: true);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartRedeployAsync(rgName, vmScaleSet.Name));

                passed = true;
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create VMSS
        /// Start VMSS
        /// Shutdown VMSS with skipShutdown = true
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetOperations_PowerOffWithSkipShutdown()
        {
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            bool passed = false;

            try
            {
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName,
                    storageAccountOutput, imageRef, createWithManagedDisks: true);
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartStartAsync(rgName, vmScaleSet.Name));
                // Shutdown VM with SkipShutdown = true
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartPowerOffAsync(rgName, vmScaleSet.Name, true));

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        [Test]
        public async Task TestVMScaleSetOperations_PerformMaintenance()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            VirtualMachineScaleSet vmScaleSet = null;

            bool passed = false;

            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "EastUS2");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName, storageAccountOutput, imageRef,
                    createWithManagedDisks: true);
                vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartPerformMaintenanceAsync(rgName, vmScaleSet.Name));

                passed = true;
            }
            catch (Exception cex)
            {
                passed = true;
                string expectedMessage =
                    $"Operation 'performMaintenance' is not allowed on VM '{vmScaleSet.Name}_0' " +
                    "since the Subscription of this VM is not eligible.";
                Assert.IsTrue(cex.Message.Contains(expectedMessage));
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet
        /// Start VMScaleSet Instances
        /// Reimage VMScaleSet Instances
        /// ReimageAll VMScaleSet Instances
        /// Stop VMScaleSet Instance
        /// ManualUpgrade VMScaleSet Instance
        /// Restart VMScaleSet Instance
        /// Deallocate VMScaleSet Instance
        /// Delete VMScaleSet Instance
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMScaleSetBatchOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            bool passed = false;
            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName: rgName,
                    vmssName: vmssName,
                    storageAccount: storageAccountOutput,
                    imageRef: imageRef,
                    createWithManagedDisks: true,
                    vmScaleSetCustomizer:
                        (virtualMachineScaleSet) => virtualMachineScaleSet.UpgradePolicy = new UpgradePolicy { Mode = UpgradeMode.Manual }
                );
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                var virtualMachineScaleSetInstanceIDs = new VirtualMachineScaleSetVMInstanceIDs( new List<string>() { "0", "1" });
                var virtualMachineScaleSetInstanceID = new List<string>() { "0", "1" };
                var virtualMachineScaleSetRequ = new VirtualMachineScaleSetVMInstanceRequiredIDs(new List<string>() { "0", "1" });
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartStartAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetInstanceIDs));
                virtualMachineScaleSetInstanceIDs = new VirtualMachineScaleSetVMInstanceIDs(new List<string>() { "0" });
                VirtualMachineScaleSetReimageParameters virtualMachineScaleSetReimageParameters = new VirtualMachineScaleSetReimageParameters
                {
                    InstanceIds = virtualMachineScaleSetInstanceID
                };
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartReimageAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetReimageParameters));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartReimageAllAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetInstanceIDs));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartPowerOffAsync(rgName, vmScaleSet.Name, null, virtualMachineScaleSetInstanceIDs));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartUpdateInstancesAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetRequ));
                virtualMachineScaleSetInstanceID = new List<string>() { "1" };
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartRestartAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetInstanceIDs));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeallocateAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetInstanceIDs));
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartDeleteInstancesAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetRequ));
                passed = true;
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        [Test]
        public async Task TestVMScaleSetBatchOperations_Redeploy()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            bool passed = false;
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "EastUS2");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName,
                    storageAccountOutput, imageRef, createWithManagedDisks: true,
                    vmScaleSetCustomizer: virtualMachineScaleSet => virtualMachineScaleSet.UpgradePolicy =
                        new UpgradePolicy { Mode = UpgradeMode.Manual });
                VirtualMachineScaleSet vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                List<string> virtualMachineScaleSetInstanceIDs = new List<string> { "0", "1" };
                var virtualMachineScaleSetInstanceID = new VirtualMachineScaleSetVMInstanceIDs(new List<string>() { "0", "1" });
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartRedeployAsync(rgName, vmScaleSet.Name, virtualMachineScaleSetInstanceID));

                passed = true;
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }

            Assert.True(passed);
        }

        [Test]
        public async Task TestVMScaleSetBatchOperations_PerformMaintenance()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            string vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;
            VirtualMachineScaleSet vmScaleSet = null;

            bool passed = false;
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "EastUS2");
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
                StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(rgName, vmssName, storageAccountOutput, imageRef,
                    createWithManagedDisks: true,
                    vmScaleSetCustomizer: virtualMachineScaleSet => virtualMachineScaleSet.UpgradePolicy =
                        new UpgradePolicy { Mode = UpgradeMode.Manual });
                vmScaleSet = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                List<string> virtualMachineScaleSetInstanceIDs = new List<string> { "0", "1" };
                var virtualMachineScaleSetInstanceID = new VirtualMachineScaleSetVMInstanceIDs(new List<string>() { "0", "1" });
                await WaitForCompletionAsync(await VirtualMachineScaleSetsClient.StartPerformMaintenanceAsync(rgName, vmScaleSet.Name,
                    virtualMachineScaleSetInstanceID));

                passed = true;
            }
            catch (Exception cex)
            {
                passed = true;
                string expectedMessage =
                    $"Operation 'performMaintenance' is not allowed on VM '{vmScaleSet.Name}_0' " +
                    "since the Subscription of this VM is not eligible.";
                Assert.IsTrue(cex.Message.Contains(expectedMessage));
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
            Assert.True(passed);
        }
    }
}
