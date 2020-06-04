﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;
using Plan = Azure.Management.Compute.Models.Plan;

namespace Azure.Management.Compute.Tests
{
    public class VMMarketplaceTest : VMTestBase
    {
        public VMMarketplaceTest(bool isAsync)
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
        public const string vmmPublisherName = "datastax";
        public const string vmmOfferName = "datastax-enterprise-non-production-use-only";
        public const string vmmSku = "sandbox_single-node";

        public async Task<VirtualMachineImage> GetMarketplaceImage()
        {
            ImageReference imageRef = await FindVMImage(vmmPublisherName, vmmOfferName, vmmSku);
            // Query the image directly in order to get all the properties, including PurchasePlan
            return await VirtualMachineImagesClient.GetAsync(m_location, vmmPublisherName, vmmOfferName, vmmSku, imageRef.Version);
        }

        [Test]
        public async Task TestVMMarketplace()
        {
            EnsureClientsInitialized();

            ImageReference dummyImageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            VirtualMachine inputVM;
            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                var img = await GetMarketplaceImage();

                Action<VirtualMachine> useVMMImage = vm =>
                {
                    vm.StorageProfile.DataDisks = null;
                    vm.StorageProfile.ImageReference = new ImageReference
                    {
                        Publisher = vmmPublisherName,
                        Offer = vmmOfferName,
                        Sku = vmmSku,
                        Version = img.Name
                    };

                    vm.Plan = new Plan()
                    {
                        Name = img.Plan.Name,
                        Product = img.Plan.Product,
                        PromotionCode = null,
                        Publisher = img.Plan.Publisher
                    };
                };

                VirtualMachine vm1 = null;
                inputVM = null;
                try
                {
                    var returnTwoVM = await CreateVM(rgName, asName, storageAccountOutput, dummyImageRef , useVMMImage);
                    vm1 = returnTwoVM.Item1;
                    inputVM = returnTwoVM.Item2;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("User failed validation to purchase resources."))
                    {
                        return;
                    }
                    throw;
                }

                // Validate the VMM Plan field
                ValidateMarketplaceVMPlanField(vm1, img);

                await WaitForCompletionAsync(await VirtualMachinesClient.StartDeleteAsync(rgName, inputVM.Name));
            }
            finally
            {
                // Don't wait for RG deletion since it's too slow, and there is nothing interesting expected with
                // the resources from this test.
                //m_ResourcesClient.ResourceGroups.BeginDelete(rgName);
            }
        }

        [Test]
        public async Task TestVMBYOL()
        {
            EnsureClientsInitialized();

            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            VirtualMachine inputVM;

            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            ImageReference dummyImageRef = null;

            // Create Storage Account, so that both the VMs can share it
            var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

            try
            {
                Action<VirtualMachine> useVMMImage = async vm =>
                {
                    vm.StorageProfile.ImageReference = await GetPlatformVMImage(true);
                    vm.LicenseType = "Windows_Server";
                };

                VirtualMachine vm1 = null;
                try
                {
                    var returnTwoVM = await CreateVM(rgName, asName, storageAccountOutput, dummyImageRef , useVMMImage);
                    vm1 = returnTwoVM.Item1;
                    inputVM = returnTwoVM.Item2;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("License type cannot be specified when creating a virtual machine from platform image. Please use an image from on-premises instead."))
                    {
                        return;
                    }
                    else if (ex.Message.Equals("Long running operation failed with status 'Failed'."))
                    {
                        return;
                    }
                    throw;
                }
                var getResponse = await VirtualMachinesClient.GetAsync(rgName, vm1.Name);
                //var getResponse = await VirtualMachinesClient.GetAsync(rgName, vm1.Name).GetAwaiter().GetResult();
                //Assert.True(getResponse.Status == HttpStatusCode.OK);
                ValidateVM(inputVM, getResponse,
                    Helpers.GetVMReferenceId(m_subId, rgName, inputVM.Name));
                var lroResponse = await WaitForCompletionAsync(await VirtualMachinesClient.StartDeleteAsync(rgName, inputVM.Name));
                //var lroResponse = await VirtualMachinesClient.DeleteWithHttpMessagesAsync(rgName, inputVM.Name).GetAwaiter().GetResult();
                /////TODO
                //Assert.True(lroResponse .StatusCode == HttpStatusCode.OK);
            }
            finally
            {
                // Don't wait for RG deletion since it's too slow, and there is nothing interesting expected with
                // the resources from this test.
                //var deleteResourceGroupResponse = m_ResourcesClient.ResourceGroups.BeginDeleteWithHttpMessagesAsync(rgName);
                //await ResourceGroupsClient.BeginDeleteWithHttpMessagesAsync(rgName);
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
                //Assert.True(deleteResourceGroupResponse.Result.Response.StatusCode == HttpStatusCode.Accepted ||
                //   deleteResourceGroupResponse.Result.Response.StatusCode == HttpStatusCode.NotFound);
            }
        }
        private void ValidateMarketplaceVMPlanField(VirtualMachine vm, VirtualMachineImage img)
        {
            Assert.NotNull(vm.Plan);
            Assert.AreEqual(img.Plan.Publisher, vm.Plan.Publisher);
            Assert.AreEqual(img.Plan.Product, vm.Plan.Product);
            Assert.AreEqual(img.Plan.Name, vm.Plan.Name);
        }
    }
}