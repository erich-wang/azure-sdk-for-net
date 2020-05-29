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
using Azure.Management.Storage.Models;

namespace Azure.Management.Compute.Tests
{
    public class ListVMTests:VMTestBase
    {
        public ListVMTests(bool isAsync)
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
        [Test]
        public async Task TestListVMInSubscription()
        {
            EnsureClientsInitialized();

            ImageReference imageRef =await GetPlatformVMImage(useWindowsImage: true);

            string baseRGName = Recording.GenerateAssetName(TestPrefix);
            string rg1Name = baseRGName + "a";
            string rg2Name = baseRGName + "b";
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1, inputVM2;

            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, asName, storageAccountOutput, imageRef);
                var vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                returnTwovm = await CreateVM(rg2Name, asName, storageAccountOutput, imageRef);
                var vm2 = returnTwovm.Item1;
                inputVM2 = returnTwovm.Item2;
                var listResponse = await (VirtualMachinesClient.ListAllAsync()).ToEnumerableAsync();
                Assert.True(listResponse.Count() >= 2);
                //Assert.Null(listResponse.NextPageLink);

                int vmsValidatedCount = 0;

                foreach (var vm in listResponse)
                {
                    if (vm.Name == vm1.Name)
                    {
                        ValidateVM(vm, vm1, Helpers.GetVMReferenceId(m_subId, rg1Name, vm1.Name));
                        vmsValidatedCount++;
                    }
                    else if (vm.Name == vm2.Name)
                    {
                        ValidateVM(vm, vm2, Helpers.GetVMReferenceId(m_subId, rg2Name, vm2.Name));
                        vmsValidatedCount++;
                    }
                }

                Assert.True(vmsValidatedCount == 2);
            }
            finally
            {
                // Cleanup the created resources. rg2 first since the VM in it needs to be deleted before the
                // storage account, which is in rg1.
                try
                {
                    await (await ResourceGroupsClient.StartDeleteAsync(rg2Name)).WaitForCompletionAsync();
                }
                finally
                {
                    await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
                }
            }
        }

        [Test]
        public async Task TestListVMsInSubscriptionByLocation()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            string baseResourceGroupName = Recording.GenerateAssetName(TestPrefix);
            string resourceGroup1Name = baseResourceGroupName + "a";
            string resourceGroup2Name = baseResourceGroupName + "b";
            string availabilitySetName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1, inputVM2;

            try
            {
                // Create Storage Account, so that both VMs can share it
                StorageAccount storageAccountOutput = await CreateStorageAccount(resourceGroup1Name, storageAccountName);

                var returnTwovm = await CreateVM(resourceGroup1Name, availabilitySetName, storageAccountOutput, imageRef);
                var vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                returnTwovm = await CreateVM(resourceGroup2Name, availabilitySetName, storageAccountOutput, imageRef);
                var vm2 = returnTwovm.Item1;
                inputVM2 = returnTwovm.Item2;
                var listResponse = await (VirtualMachinesClient.ListByLocationAsync(DefaultLocation)).ToEnumerableAsync();
                Assert.True(listResponse.Count() >= 2);
                //Assert.Null(listResponse.NextPageLink);

                int vmsValidatedCount = 0;

                foreach (VirtualMachine vm in listResponse)
                {
                    if (vm.Name.Equals(vm1.Name))
                    {
                        ValidateVM(vm, vm1, Helpers.GetVMReferenceId(m_subId, resourceGroup1Name, vm1.Name));
                        vmsValidatedCount++;
                    }
                    else if (vm.Name.Equals(vm2.Name))
                    {
                        ValidateVM(vm, vm2, Helpers.GetVMReferenceId(m_subId, resourceGroup2Name, vm2.Name));
                        vmsValidatedCount++;
                    }
                }

                Assert.AreEqual(2, vmsValidatedCount);
            }
            finally
            {
                // Cleanup the created resources. rg2 first since the VM in it needs to be deleted before the
                // storage account, which is in rg1.
                try
                {
                    await (await ResourceGroupsClient.StartDeleteAsync(resourceGroup2Name)).WaitForCompletionAsync();
                }
                finally
                {
                    await (await ResourceGroupsClient.StartDeleteAsync(resourceGroup1Name)).WaitForCompletionAsync();
                }
            }

        }
    }
}
