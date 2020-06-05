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
    public class DedicatedHostTests:VMTestBase
    {

        public DedicatedHostTests(bool isAsync )
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
        public async Task TestDedicatedHostOperations()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
            EnsureClientsInitialized();

            string baseRGName = Recording.GenerateAssetName(TestPrefix);
            string rgName = baseRGName + "DH";
            string dhgName = "DHG-1";
            string dhName = "DH-1";
            try
            {
                // Create a dedicated host group, then get the dedicated host group and validate that they match
                DedicatedHostGroup createdDHG = await CreateDedicatedHostGroup(rgName, dhgName);
                DedicatedHostGroup returnedDHG = await DedicatedHostGroupsClient.GetAsync(rgName, dhgName);
                ValidateDedicatedHostGroup(createdDHG, returnedDHG);

                // Update existing dedicated host group
                DedicatedHostGroupUpdate updateDHGInput = new DedicatedHostGroupUpdate()
                {
                    Tags = new Dictionary<string, string>() { { "testKey", "testValue" } }
                };
                createdDHG.Tags = updateDHGInput.Tags;
                updateDHGInput.PlatformFaultDomainCount = returnedDHG.PlatformFaultDomainCount; // There is a bug in PATCH.  PlatformFaultDomainCount is a required property now.
                returnedDHG = await DedicatedHostGroupsClient.UpdateAsync(rgName, dhgName, updateDHGInput);
                ValidateDedicatedHostGroup(createdDHG, returnedDHG);

                //List DedicatedHostGroups by subscription and by resourceGroup
                var listDHGsResponse = DedicatedHostGroupsClient.ListByResourceGroupAsync(rgName);
                var listDHGsResponseRes =await listDHGsResponse.ToEnumerableAsync();
                Assert.IsTrue(listDHGsResponseRes.Count()==1);
                //Assert.Single(listDHGsResponse);
                ValidateDedicatedHostGroup(createdDHG, listDHGsResponseRes.First());
                listDHGsResponse = DedicatedHostGroupsClient.ListBySubscriptionAsync();
                listDHGsResponseRes = await listDHGsResponse.ToEnumerableAsync();
                //There might be multiple dedicated host groups in the subscription, we only care about the one that we created.
                returnedDHG = listDHGsResponseRes.First(dhg => dhg.Id == createdDHG.Id);
                Assert.NotNull(returnedDHG);
                ValidateDedicatedHostGroup(createdDHG, returnedDHG);

                //Create DedicatedHost within the DedicatedHostGroup and validate
                var createdDH =await CreateDedicatedHost(rgName, dhgName, dhName);
                var returnedDH = await DedicatedHostsClient.GetAsync(rgName, dhgName, dhName);
                ValidateDedicatedHost(createdDH, returnedDH);

                //List DedicatedHosts
                var listDHsResponse = DedicatedHostsClient.ListByHostGroupAsync(rgName, dhgName);
                var listDHsResponseRes = await listDHsResponse.ToEnumerableAsync();
                Assert.IsTrue(listDHsResponseRes.Count()==1);
                ValidateDedicatedHost(createdDH, listDHsResponseRes.First());

                //Delete DedicatedHosts and DedicatedHostGroups
                await WaitForCompletionAsync(await DedicatedHostsClient.StartDeleteAsync(rgName, dhgName, dhName));
                await DedicatedHostGroupsClient.DeleteAsync(rgName, dhgName);
                WaitMinutes(1);
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        private void ValidateDedicatedHostGroup(DedicatedHostGroup expectedDHG, DedicatedHostGroup actualDHG)
        {
            if (expectedDHG == null)
            {
                Assert.Null(actualDHG);
            }
            else
            {
                Assert.NotNull(actualDHG);
                if (expectedDHG.Hosts == null)
                {
                    Assert.Null(actualDHG.Hosts);
                }
                else
                {
                    Assert.NotNull(actualDHG);
                    Assert.True(actualDHG.Hosts.SequenceEqual(expectedDHG.Hosts));
                }
                Assert.AreEqual(expectedDHG.Location, actualDHG.Location);
                Assert.AreEqual(expectedDHG.Name, actualDHG.Name);
            }

        }

        private void ValidateDedicatedHost(DedicatedHost expectedDH, DedicatedHost actualDH)
        {
            if (expectedDH == null)
            {
                Assert.Null(actualDH);
            }
            else
            {
                Assert.NotNull(actualDH);
                if (expectedDH.VirtualMachines == null)
                {
                    Assert.Null(actualDH.VirtualMachines);
                }
                else
                {
                    Assert.NotNull(actualDH);
                    Assert.True(actualDH.VirtualMachines.SequenceEqual(expectedDH.VirtualMachines));
                }
                Assert.AreEqual(expectedDH.Location, actualDH.Location);
                Assert.AreEqual(expectedDH.Name, actualDH.Name);
                Assert.AreEqual(expectedDH.HostId, actualDH.HostId);
            }
        }
    }
}
