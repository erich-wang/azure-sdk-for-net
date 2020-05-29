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
    public class VMDiskEncryptionTests:VMTestBase
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
            //ComputeManagementClient computeClient;
            //ResourceManagementClient resourcesClient;
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
        //[Trait("Name", "TestDiskEncryption")]
        public async Task TestVMDiskEncryption()
        {

            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");

            try
            {
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
                await VirtualMachinesClient.StartDeleteAsync(rgName, inputVM1.Name);
                await VirtualMachinesClient.StartDeleteAsync(rgName, inputVM2.Name);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }
    }
}
