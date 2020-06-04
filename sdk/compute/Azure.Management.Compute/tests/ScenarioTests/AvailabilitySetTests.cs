﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class AvailabilitySetTests : VMTestBase
    {
        public ResourceGroup resourceGroup1;
        public ResourceGroup resourceGroup2;

        public string subId;
        public string baseResourceGroupName;
        public string resourceGroup1Name;

        // These values are configurable in the service, but normal default values are FD = 3 and UD = 5
        // FD values can be 2 or 3
        // UD values are 1 -> 20
        public const int nonDefaultFD = 2;
        public const int nonDefaultUD = 4;

        public const int defaultFD = 3;
        public const int defaultUD = 5;

        // These constants for for the out of range tests
        public const int FDTooLow = 0;
        public const int FDTooHi = 4;
        public const int UDTooLow = 0;
        public const int UDTooHi = 21;
        public AvailabilitySetTests(bool isAsync)
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
        public async Task TestOperations()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "westcentralus");
                EnsureClientsInitialized();

                Initialize();
                //subId = subscriptionId;
                //location = m_location;


                //baseResourceGroupName = Recording.GenerateAssetName(TestPrefix);
                //resourceGroup1Name = baseResourceGroupName + "_1";

                //resourceGroup1 = await ResourceGroupsClient.CreateOrUpdateAsync(
                //    resourceGroup1Name,
                //    new ResourceGroup(location)
                //    {
                //        Tags = new Dictionary<string, string>() { { resourceGroup1Name, DateTime.UtcNow.ToString("u") } }
                //    });
                // Attempt to Create Availability Set with out of bounds FD and UD values
                VerifyInvalidFDUDValuesFail();

                // Create a Availability Set with default values
                VerifyDefaultValuesSucceed();

                // Make sure non default FD and UD values succeed
                VerifyNonDefaultValuesSucceed();

                // Updating an Availability Set should fail
                //VerifyUpdateFails();

                // Make sure availability sets across resource groups are listed successfully
                VerifyListAvailabilitySetsInSubscription();
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                await WaitForCompletionAsync( await ResourceGroupsClient.StartDeleteAsync(resourceGroup1Name));
            }
        }

        private async void VerifyInvalidFDUDValuesFail()
        {
            var inputAvailabilitySetName = Recording.GenerateAssetName("invalidfdud");
            var inputAvailabilitySet = new AvailabilitySet(TestEnvironment.Location)
            {
                Tags = new Dictionary<string, string>()
                    {
                        {"RG", "rg"},
                        {"testTag", "1"},
                    },
            };

            // function to test the limits available.
            inputAvailabilitySet.PlatformFaultDomainCount = FDTooLow;
            AvailabilitySet createOrUpdateResponse = null;
            try
            {
                createOrUpdateResponse = await AvailabilitySetsClient.CreateOrUpdateAsync(
                    resourceGroup1Name,
                    inputAvailabilitySetName,
                    inputAvailabilitySet);
            }
            catch (Exception ex)
            //catch (CloudException ex)
            {
                Assert.NotNull(ex);
                //Assert.True(ex.Response.StatusCode == HttpStatusCode.BadRequest);
            }
            Assert.True(createOrUpdateResponse == null);

            inputAvailabilitySet.PlatformFaultDomainCount = FDTooHi;
            try
            {
                createOrUpdateResponse = await AvailabilitySetsClient.CreateOrUpdateAsync(
                    resourceGroup1Name,
                    inputAvailabilitySetName,
                    inputAvailabilitySet);
            }
            catch ( Exception ex)
            {
                Assert.NotNull(ex);
                //Assert.True(ex.Response.StatusCode == HttpStatusCode.BadRequest);

            }
            Assert.True(createOrUpdateResponse == null);

            inputAvailabilitySet.PlatformUpdateDomainCount = UDTooLow;
            try
            {
                createOrUpdateResponse = await AvailabilitySetsClient.CreateOrUpdateAsync(
                    resourceGroup1Name,
                    inputAvailabilitySetName,
                    inputAvailabilitySet);
            }
            catch ( Exception ex)
            {
                Assert.NotNull(ex);
                //Assert.True(ex.Response.StatusCode == HttpStatusCode.BadRequest);

            }
            Assert.True(createOrUpdateResponse == null);

            inputAvailabilitySet.PlatformUpdateDomainCount = UDTooHi;
            try
            {
                createOrUpdateResponse =await  AvailabilitySetsClient.CreateOrUpdateAsync(
                resourceGroup1Name,
                inputAvailabilitySetName,
                inputAvailabilitySet);
            }
            catch ( Exception ex)
            {
                Assert.NotNull(ex);
                //Assert.True(ex.Response.StatusCode == HttpStatusCode.BadRequest);

            }
            Assert.True(createOrUpdateResponse == null);
        }

        private async void VerifyDefaultValuesSucceed()
        {
            var inputAvailabilitySetName = Recording.GenerateAssetName("asdefaultvalues");
            var inputAvailabilitySet = new AvailabilitySet(TestEnvironment.Location)
            {
                Tags = new Dictionary<string, string>()
                    {
                        {"RG", "rg"},
                        {"testTag", "1"},
                    },
            };

            var createOrUpdateResponse = await AvailabilitySetsClient.CreateOrUpdateAsync(
                resourceGroup1Name,
                inputAvailabilitySetName,
                inputAvailabilitySet);

            // List AvailabilitySets
            string expectedAvailabilitySetId = Helpers.GetAvailabilitySetRef(subId, resourceGroup1Name, inputAvailabilitySetName);
            var listResponse = AvailabilitySetsClient.ListAsync(resourceGroup1Name);
            var listResponseList = await listResponse.ToEnumerableAsync();
            ValidateAvailabilitySet(inputAvailabilitySet, listResponseList.FirstOrDefault(x => x.Name == inputAvailabilitySetName),
                inputAvailabilitySetName, expectedAvailabilitySetId, defaultFD, defaultUD);

            AvailabilitySetUpdate updateParams = new AvailabilitySetUpdate()
            {
                Tags = inputAvailabilitySet.Tags
            };

            string updateKey = "UpdateTag";
            updateParams.Tags.Add(updateKey, "updateValue");
            createOrUpdateResponse = await AvailabilitySetsClient.UpdateAsync(resourceGroup1Name, inputAvailabilitySetName, updateParams);

            Assert.True(createOrUpdateResponse.Value.Tags.ContainsKey(updateKey));

            // This call will also delete the Availability Set
            ValidateResults(createOrUpdateResponse, inputAvailabilitySet, resourceGroup1Name, inputAvailabilitySetName, defaultFD, defaultUD);
        }

        private async void VerifyNonDefaultValuesSucceed()
        {
            // Negative tests for a bug in 5.0.0 that read-only fields have side-effect on the request body
            var testStatus = new InstanceViewStatus
            {
                Code = "test",
                DisplayStatus = "test",
                Message = "test"
            };

            string inputAvailabilitySetName = Recording.GenerateAssetName("asnondefault");
            var inputAvailabilitySet = new AvailabilitySet(TestEnvironment.Location)
            {
                Tags = new Dictionary<string, string>()
                    {
                        {"RG", "rg"},
                        {"testTag", "1"},
                    },
                PlatformFaultDomainCount = nonDefaultFD,
                PlatformUpdateDomainCount = nonDefaultUD
            };

            var createOrUpdateResponse = await AvailabilitySetsClient.CreateOrUpdateAsync(
                resourceGroup1Name,
                inputAvailabilitySetName,
                inputAvailabilitySet);

            // This call will also delete the Availability Set
            ValidateResults(createOrUpdateResponse, inputAvailabilitySet, resourceGroup1Name, inputAvailabilitySetName, nonDefaultFD, nonDefaultUD);
        }
        private async void ValidateResults(AvailabilitySet outputAvailabilitySet, AvailabilitySet inputAvailabilitySet, string resourceGroupName, string inputAvailabilitySetName, int expectedFD, int expectedUD)
        {
            string expectedAvailabilitySetId = Helpers.GetAvailabilitySetRef(subId, resourceGroupName, inputAvailabilitySetName);

            Assert.True(outputAvailabilitySet.Name == inputAvailabilitySetName);
            Assert.True(outputAvailabilitySet.Location.ToLower() == this.TestEnvironment.Location.ToLower()
                     || outputAvailabilitySet.Location.ToLower() == inputAvailabilitySet.Location.ToLower());

            ValidateAvailabilitySet(inputAvailabilitySet, outputAvailabilitySet, inputAvailabilitySetName, expectedAvailabilitySetId, expectedFD, expectedUD);

            // GET AvailabilitySet
            var getResponse = await AvailabilitySetsClient.GetAsync(resourceGroupName, inputAvailabilitySetName);
            ValidateAvailabilitySet(inputAvailabilitySet, getResponse, inputAvailabilitySetName, expectedAvailabilitySetId, expectedFD, expectedUD);

            // List VM Sizes
            var listVMSizesResponse = AvailabilitySetsClient.ListAvailableSizesAsync(resourceGroupName, inputAvailabilitySetName);
            var listVMSizesResp = await listVMSizesResponse.ToEnumerableAsync();
            Helpers.ValidateVirtualMachineSizeListResponse(listVMSizesResp);

            // Delete AvailabilitySet
            await AvailabilitySetsClient.DeleteAsync(resourceGroupName, inputAvailabilitySetName);
        }

        private void ValidateAvailabilitySet(AvailabilitySet inputAvailabilitySet, AvailabilitySet outputAvailabilitySet, string inputAvailabilitySetName, string expectedAvailabilitySetId, int expectedFD, int expectedUD)
        {
            Assert.True(inputAvailabilitySetName == outputAvailabilitySet.Name);
            Assert.True(outputAvailabilitySet.Type == ApiConstants.ResourceProviderNamespace + "/" + ApiConstants.AvailabilitySets);

            Assert.True(outputAvailabilitySet != null);
            Assert.True(outputAvailabilitySet.PlatformFaultDomainCount == expectedFD);
            Assert.True(outputAvailabilitySet.PlatformUpdateDomainCount == expectedUD);

            Assert.NotNull(inputAvailabilitySet.Tags);
            Assert.NotNull(outputAvailabilitySet.Tags);

            foreach (var tag in inputAvailabilitySet.Tags)
            {
                string key = tag.Key;
                Assert.True(inputAvailabilitySet.Tags[key] == outputAvailabilitySet.Tags[key]);
            }
            // TODO: Dev work corresponding to setting status is not yet checked in.
            //Assert.NotNull(outputAvailabilitySet.Properties.Id);
            //Assert.True(expectedAvailabilitySetIds.ToLowerInvariant() == outputAvailabilitySet.Properties.Id.ToLowerInvariant());
        }

        // Make sure availability sets across resource groups are listed successfully
        private async void VerifyListAvailabilitySetsInSubscription()
        {
            string resourceGroup2Name = baseResourceGroupName + "_2";
            string baseInputAvailabilitySetName = Recording.GenerateAssetName("asdefaultvalues");
            string availabilitySet1Name = baseInputAvailabilitySetName + "_1";
            string availabilitySet2Name = baseInputAvailabilitySetName + "_2";

            try
            {
                AvailabilitySet inputAvailabilitySet1 = new AvailabilitySet(TestEnvironment.Location)
                {
                    Tags = new Dictionary<string, string>()
                    {
                        {"RG1", "rg1"},
                        {"testTag", "1"},
                    },
                };
                AvailabilitySet outputAvailabilitySet1 = await AvailabilitySetsClient.CreateOrUpdateAsync(
                    resourceGroup1Name,
                    availabilitySet1Name,
                    inputAvailabilitySet1);

                resourceGroup2 = await ResourceGroupsClient.CreateOrUpdateAsync(
                    resourceGroup2Name,
                    new ResourceGroup(TestEnvironment.Location)
                    {
                        Tags = new Dictionary<string, string>() { { resourceGroup2Name, DateTime.UtcNow.ToString("u") } }
                    });

                AvailabilitySet inputAvailabilitySet2 = new AvailabilitySet(TestEnvironment.Location)
                {
                    Tags = new Dictionary<string, string>()
                    {
                        {"RG2", "rg2"},
                        {"testTag", "2"},
                    },
                };
                AvailabilitySet outputAvailabilitySet2 = await AvailabilitySetsClient.CreateOrUpdateAsync(
                    resourceGroup2Name,
                    availabilitySet2Name,
                    inputAvailabilitySet2);
                var response = AvailabilitySetsClient.ListBySubscriptionAsync();
                var resp = await response.ToEnumerableAsync();
                //Assert.Null(resp.NextPageLink);

                foreach (AvailabilitySet availabilitySet in resp)
                {
                    if (availabilitySet.Name == availabilitySet1Name)
                    {
                        Assert.AreEqual(inputAvailabilitySet1.Location, availabilitySet.Location);
                        Assert.Null(availabilitySet.VirtualMachines);
                    }
                    else if (availabilitySet.Name == availabilitySet2Name)
                    {
                        Assert.AreEqual(inputAvailabilitySet2.Location, availabilitySet.Location);
                        Assert.Null(availabilitySet.VirtualMachines);
                    }
                }

                response = AvailabilitySetsClient.ListBySubscriptionAsync("virtualMachines/$ref");
                resp = await response.ToEnumerableAsync();
                int validationCount = 0;

                foreach (AvailabilitySet availabilitySet in resp)
                {
                    Assert.NotNull(availabilitySet.VirtualMachines);
                    if (availabilitySet.Name == availabilitySet1Name)
                    {
                        Assert.AreEqual(0, availabilitySet.VirtualMachines.Count);
                        ValidateResults(outputAvailabilitySet1, inputAvailabilitySet1, resourceGroup1Name, availabilitySet1Name, defaultFD, defaultUD);
                        validationCount++;
                    }
                    else if (availabilitySet.Name == availabilitySet2Name)
                    {
                        Assert.AreEqual(0, availabilitySet.VirtualMachines.Count);
                        ValidateResults(outputAvailabilitySet2, inputAvailabilitySet2, resourceGroup2Name, availabilitySet2Name, defaultFD, defaultUD);
                        validationCount++;
                    }
                }

                Assert.True(validationCount == 2);
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(resourceGroup2Name));
            }
        }

        private async void Initialize()
        {
            subId = TestEnvironment.SubscriptionId;
            //.Location = m_location;

            baseResourceGroupName = Recording.GenerateAssetName(TestPrefix);
            resourceGroup1Name = baseResourceGroupName + "_1";

            resourceGroup1 = await ResourceGroupsClient.CreateOrUpdateAsync(
                resourceGroup1Name,
                new ResourceGroup(TestEnvironment.Location)
                {
                    Tags = new Dictionary<string, string>() { { resourceGroup1Name, DateTime.UtcNow.ToString("u") } }
                });
        }
    }
}