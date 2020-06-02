// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Core.TestFramework;
    using Azure.Management.EventHub.Models;
    using NUnit.Framework;
    public partial class ScenarioTests : EventHubManagementClientBase
    {
        [Test]
        public async Task ConsumerGroupsCreateGetUpdateDelete_Length()
        {
            var location = GetLocation();
            var resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
            await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient, location.Result, resourceGroup);
            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);
            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location.Result,
                    Sku = new Sku(SkuName.Standard)
                    {
                        Tier = SkuTier.Standard
                    },
                    Tags = new Dictionary<string, string>()
                    {
                        {"tag1", "value1"},
                        {"tag2", "value2"}
                    }
                }
                );
            var np = (await WaitForCompletionAsync(createNamespaceResponse)).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np.Name, namespaceName);
            IsDelay(5);
            // Create Eventhub
            var eventhubName = Helper.EventHubPrefix + "thisisthenamewithmorethan53charschecktoverifytheremovlaof50charsnamelengthlimit";
            var createEventhubResponse = await EventHubsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubName,
            new Eventhub() { MessageRetentionInDays = 5 });
            Assert.NotNull(createEventhubResponse);
            Assert.AreEqual(createEventhubResponse.Value.Name, eventhubName);
            //Get the created EventHub
            var geteventhubResponse = await EventHubsClient.GetAsync(resourceGroup, namespaceName, eventhubName);
            Assert.NotNull(geteventhubResponse);
            Assert.AreEqual(EntityStatus.Active, geteventhubResponse.Value.Status);
            Assert.AreEqual(geteventhubResponse.Value.Name, eventhubName);
            // Create ConsumerGroup.
            var consumergroupName = "thisisthenamewithmorethan53charschecktoverifqwert";
            string UserMetadata = "Newly Created";
            var createConsumergroupResponse =await ConsumerGroupsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubName, consumergroupName, new ConsumerGroup { UserMetadata = UserMetadata });
            Assert.NotNull(createConsumergroupResponse);
            Assert.AreEqual(createConsumergroupResponse.Value.Name, consumergroupName);
            // Get Created ConsumerGroup
            var getConsumergroupGetResponse = ConsumerGroupsClient.GetAsync(resourceGroup, namespaceName, eventhubName, consumergroupName);
            Assert.NotNull(getConsumergroupGetResponse);
            Assert.AreEqual(getConsumergroupGetResponse.Result.Value.Name, consumergroupName);
            // Get all ConsumerGroup
            var getSubscriptionsListAllResponse = ConsumerGroupsClient.ListByEventHubAsync(resourceGroup, namespaceName, eventhubName);
            Assert.NotNull(getSubscriptionsListAllResponse);
            bool isContainresourceGroup = false;
            var list = await getSubscriptionsListAllResponse.ToEnumerableAsync();
            foreach (var detail in list)
            {
                if (detail.Id.Contains(resourceGroup))
                {
                    isContainresourceGroup = true;
                    break;
                }
            }
            Assert.True(isContainresourceGroup);
            //Assert.True(getSubscriptionsListAllResponse.All(ns => ns.Id.Contains(resourceGroup)));
            //Update the Created consumergroup
            createConsumergroupResponse.Value.UserMetadata = "Updated the user meta data";
            var updateconsumergroupResponse =await ConsumerGroupsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, eventhubName, consumergroupName, createConsumergroupResponse);
            Assert.NotNull(updateconsumergroupResponse);
            Assert.AreEqual(updateconsumergroupResponse.Value.Name, createConsumergroupResponse.Value.Name);
            Assert.AreEqual("Updated the user meta data", updateconsumergroupResponse.Value.UserMetadata);
            // Get Created ConsumerGroup
            var getConsumergroupResponse =await ConsumerGroupsClient.GetAsync(resourceGroup, namespaceName, eventhubName, consumergroupName);
            Assert.NotNull(getConsumergroupResponse);
            Assert.AreEqual(getConsumergroupResponse.Value.Name, consumergroupName);
            Assert.AreEqual(getConsumergroupResponse.Value.UserMetadata, updateconsumergroupResponse.Value.UserMetadata);
            // Delete Created ConsumerGroup and check for the NotFound exception
            await ConsumerGroupsClient.DeleteAsync(resourceGroup, namespaceName, eventhubName, consumergroupName);
            // Delete Created EventHub  and check for the NotFound exception
            await EventHubsClient.DeleteAsync(resourceGroup, namespaceName, eventhubName);
            // Delete namespace
            await WaitForCompletionAsync(await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName));
            //Subscription end
        }
    }
}
