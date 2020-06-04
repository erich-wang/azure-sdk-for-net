﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class VMDiskSizeTests : VMTestBase
    {
        public VMDiskSizeTests(bool isAsync)
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
        [Test]
        public async Task TestVMDiskSizeScenario()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            var image = await VirtualMachineImagesClient.GetAsync(
                this.m_location, imageRef.Publisher, imageRef.Offer, imageRef.Sku, imageRef.Version);
            Assert.True(image != null);

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            VirtualMachine inputVM;

            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var returnTwoVM = await CreateVM(rgName, asName, storageAccountOutput, imageRef, (vm) =>
                 {
                     vm.StorageProfile.OsDisk.DiskSizeGB = 150;
                 });
                var vm1 = returnTwoVM.Item1;
                inputVM = returnTwoVM.Item2;
                var getVMResponse = await VirtualMachinesClient.GetAsync(rgName, inputVM.Name);
                ValidateVM(inputVM, getVMResponse, Helpers.GetVMReferenceId(m_subId, rgName, inputVM.Name));
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
    }
}
