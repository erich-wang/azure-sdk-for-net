﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.ResourceManager.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;

namespace Azure.ResourceManager.Compute.Tests
{
<<<<<<< HEAD
=======
    [AsyncOnly]
>>>>>>> erichmaster/track2/compute.tests
    public class VMDiskEncryptionTests : VMTestBase
    {
        public VMDiskEncryptionTests(bool isAsync)
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
        }

        [TearDown]
        public async Task CleanupResourceGroup()
        {
            await CleanupResourceGroupsAsync();
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VM with DiskEncryptionSettings
        /// GET VM Model View
        /// Delete VM
        /// Delete RG
        /// TODO: Add negative test case validation
        /// </summary>
        [Test]
        [Ignore("this should be tested by generate team")]
        //[Trait("Name", "TestDiskEncryption")]
        public async Task TestVMDiskEncryption()
        {

<<<<<<< HEAD
            EnsureClientsInitialized();
=======
            EnsureClientsInitialized(true);
>>>>>>> erichmaster/track2/compute.tests

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");

            // Create Storage Account, so that both the VMs can share it
            var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);
            //Create VM with encryptionKey
            VirtualMachine inputVM1;
            var returnTwoVm = await CreateVM(rgName, asName, storageAccountOutput, imageRef,
                (vm) =>
                {
                    vm.StorageProfile.OsDisk.EncryptionSettings = GetEncryptionSettings();
                    vm.HardwareProfile.VmSize = VirtualMachineSizeTypes.StandardD1;
                }, waitForCompletion: false);
            inputVM1 = returnTwoVm.Item2;
            //Create VM with encryptionKey and KEK
            VirtualMachine inputVM2;
            returnTwoVm = await CreateVM(rgName, asName, storageAccountOutput, imageRef,
                (vm) =>
                {
                    vm.StorageProfile.OsDisk.EncryptionSettings = GetEncryptionSettings(addKek: true);
                    vm.HardwareProfile.VmSize = VirtualMachineSizeTypes.StandardD1;
                }, waitForCompletion: false);
            inputVM2 = returnTwoVm.Item2;
            await WaitForCompletionAsync(await VirtualMachinesOperations.StartDeleteAsync(rgName, inputVM1.Name));
            await WaitForCompletionAsync(await VirtualMachinesOperations.StartDeleteAsync(rgName, inputVM2.Name));
        }
    }
}