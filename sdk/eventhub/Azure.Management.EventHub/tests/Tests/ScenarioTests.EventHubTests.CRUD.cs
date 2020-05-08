﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.EventHub.Models;
using Azure.Management.Resource;
using NUnit.Framework;

    public partial class ScenarioTests : EventHubManagementClientBase
    {
        public ScenarioTests(bool isAsync, EventHubManagementClientOption.ServiceVersion serviceVersion)
            : base(isAsync, serviceVersion)
        {
        }

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                InitializeClients();
                //Client = GetHubsClient();
                //namespacesClient = GetNamespacesClient();
                //ChallengeBasedAuthenticationPolicy.AuthenticationChallenge.ClearCache();
            }
        }

        [Test]
        public async Task EventCreateGetUpdateDelete()
        {
            var location = GetLocation();
            var resourceGroup = TryGetResourceGroup(ResourceGroupsClient, location.Result);
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
                await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient,location.Result, resourceGroup);
            }

            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);

            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location.Result
                    //Sku = new Sku("as")
                }
                );
            var np = (await createNamespaceResponse.WaitForCompletionAsync()).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np.Name, namespaceName);
            isDelay(5);

            // Create Eventhub
            var eventhubName = Recording.GenerateAssetName(Helper.EventHubPrefix);

            //You Need to create a storage account first --Youri 8.5.2020
            var createEventhubResponse = await EventHubsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubName,
                new Eventhub()
                {
                    MessageRetentionInDays = 4,
                    PartitionCount = 4,
                    Status = EntityStatus.Active,
                    CaptureDescription = new CaptureDescription()
                    {
                        Enabled = true,
                        Encoding = EncodingCaptureDescription.Avro,
                        IntervalInSeconds = 120,
                        SizeLimitInBytes = 10485763,
                        Destination = new Destination()
                        {
                            Name = "EventHubArchive.AzureBlockBlob",
                            BlobContainer = "container",
                            ArchiveNameFormat = "{Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}",
                            StorageAccountResourceId = "/subscriptions/" + subscriptionId + "/resourcegroups/"+resourceGroup+"/providers/Microsoft.Storage/storageAccounts/testingsdkeventhub88"
                        },
                        SkipEmptyArchives = true
                    }
                });
            Assert.NotNull(createEventhubResponse);
            Assert.AreEqual(createEventhubResponse.Value.Name, eventhubName);
            Assert.True(createEventhubResponse.Value.CaptureDescription.SkipEmptyArchives);

            //Get the created EventHub
            var getEventHubResponse = await EventHubsClient.GetAsync(resourceGroup, namespaceName, eventhubName);
            Assert.NotNull(getEventHubResponse.Value);
            Assert.AreEqual(EntityStatus.Active,getEventHubResponse.Value.Status);
            Assert.AreEqual(getEventHubResponse.Value.Name, eventhubName);

            //Get all Event Hubs for a given NameSpace
            var getListEventHubResponse = EventHubsClient.ListByNamespaceAsync(resourceGroup, namespaceName);
            var list = await getListEventHubResponse.ToEnumerableAsync();
            Assert.NotNull(getListEventHubResponse);
            Assert.True(list.Count()>=1);

            // Update the EventHub
            getEventHubResponse.Value.CaptureDescription.IntervalInSeconds = 130;
            getEventHubResponse.Value.CaptureDescription.SizeLimitInBytes = 10485900;
            getEventHubResponse.Value.MessageRetentionInDays = 5;
            //TODO time exception
            var UpdateEventHubResponse = (await EventHubsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubName, getEventHubResponse.Value)).Value;
            Assert.NotNull(UpdateEventHubResponse);

            // Get the updated EventHub and verify the properties
            var getEventResponse = await EventHubsClient.GetAsync(resourceGroup, namespaceName, eventhubName);
            Assert.NotNull(getEventResponse);
            Assert.AreEqual(EntityStatus.Active, getEventResponse.Value.Status);
            Assert.AreEqual(5, getEventResponse.Value.MessageRetentionInDays);


            // Delete the Evnet Hub
            var deleteEventResponse = await EventHubsClient.DeleteAsync(resourceGroup, namespaceName, eventhubName);
            // Delete namespace and check for the NotFound exception
            var deleteNamespaceResponse = await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName);
        }
    }
}
