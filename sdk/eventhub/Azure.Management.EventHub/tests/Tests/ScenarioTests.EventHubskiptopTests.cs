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
using NUnit.Framework;

    public partial class ScenarioTests : EventHubManagementClientBase
    {


        [Test]
        public async Task EventHubskiptop()
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
            var eventHubName = Recording.GenerateAssetName(Helper.EventHubPrefix);

            //Assert.NotNull(createEventHubResponse);
            //Assert.Equals(createEventHubResponse.Value.Name, eventHubName);
            //Assert.True(createEventHubResponse.Value.CaptureDescription.SkipEmptyArchives);
            for (int ehCount = 0; ehCount < 10; ehCount++)
            {
                var eventhubNameLoop = eventHubName + "_" + ehCount.ToString();
                var createEventHubResponseForLoop = EventHubsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubNameLoop, new Eventhub());

                Assert.NotNull(createEventHubResponseForLoop);
                Assert.AreEqual(createEventHubResponseForLoop.Result.Value.Name, eventhubNameLoop);
            }

            //get EventHubs in the same namespace
            var createEventHubResponseList = EventHubsClient.ListByNamespaceAsync(resourceGroup, namespaceName);
            var createEHResplist = await createEventHubResponseList.ToEnumerableAsync();
            //may cause a misktake
            Assert.AreEqual(10, createEHResplist.Count());

            var gettop10EventHub = EventHubsClient.ListByNamespaceAsync(resourceGroup, namespaceName, skip: 5, top: 5);
            var ListByNamespAsync = await gettop10EventHub.ToEnumerableAsync();
            Assert.AreEqual(5, ListByNamespAsync.Count());

            // Create a ConsumerGroup
            var consumergroupName = Recording.GenerateAssetName(Helper.ConsumerGroupPrefix);
            for (int consumergroupCount = 0; consumergroupCount < 10; consumergroupCount++)
            {
                var consumergroupNameLoop = consumergroupName + "_" + consumergroupCount.ToString();
                var createConsumerGroupResponseForLoop =await ConsumerGroupsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, createEHResplist.ElementAt<Eventhub>(0).Name, consumergroupNameLoop, new ConsumerGroup());
                Assert.NotNull(createConsumerGroupResponseForLoop);
                Assert.AreEqual(createConsumerGroupResponseForLoop.Value.Name, consumergroupNameLoop);
            }

            var createConsumerGroupResponseList = ConsumerGroupsClient.ListByEventHubAsync(resourceGroup, namespaceName, createEHResplist.ElementAt<Eventhub>(0).Name);
            var createConResList = await createConsumerGroupResponseList.ToEnumerableAsync();
            Assert.AreEqual(11, createConResList.Count<ConsumerGroup>());

            var gettop10ConsumerGroup = ConsumerGroupsClient.ListByEventHubAsync(resourceGroup, namespaceName, createEHResplist.ElementAt<Eventhub>(0).Name, skip: 5, top: 4);
            var ConsGrClientList = await gettop10ConsumerGroup.ToEnumerableAsync();
            Assert.AreEqual(6, ConsGrClientList.Count<ConsumerGroup>());
            //isDelay();
            // Delete namespace and check for the NotFound exception
            await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName);
        }
    }
}
