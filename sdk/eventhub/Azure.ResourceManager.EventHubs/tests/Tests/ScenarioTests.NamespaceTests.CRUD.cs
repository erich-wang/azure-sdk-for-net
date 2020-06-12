// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Core.TestFramework;
    using Azure.ResourceManager.EventHubs.Models;
    using Azure.ResourceManager.EventHubs.Tests;
    using NUnit.Framework;
    public partial class ScenarioTests : EventHubsManagementClientBase
    {
        [Test]
        public async Task NamespaceCreateGetUpdateDelete()
        {
            var location = GetLocation();
            var resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
            await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient, location.Result, resourceGroup);
            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);
            var operationsResponse = OperationsClient.ListAsync();
            var checkNameAvailable = NamespacesClient.CheckNameAvailabilityAsync(new CheckNameAvailabilityParameter(namespaceName));
            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location.Result
                }
                );
            var np = (await WaitForCompletionAsync(createNamespaceResponse)).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np.Name,namespaceName);
            IsDelay(60);
            //get the created namespace
            var getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            if (string.Compare(getNamespaceResponse.Value.ProvisioningState, "Succeeded", true) != 0)
                IsDelay(10);
            getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            Assert.NotNull(getNamespaceResponse);
            Assert.AreEqual("Succeeded", getNamespaceResponse.Value.ProvisioningState,StringComparer.CurrentCultureIgnoreCase.ToString());
            Assert.AreEqual(location.Result, getNamespaceResponse.Value.Location);
            // Get all namespaces created within a resourceGroup
            var getAllNamespacesResponse =  NamespacesClient.ListByResourceGroupAsync(resourceGroup);
            Assert.NotNull(getAllNamespacesResponse);
            //Assert.True(getAllNamespacesResponse.AsPages.c >= 1);
            bool isContainnamespaceName = false;
            bool isContainresourceGroup = false;
            var list = await getAllNamespacesResponse.ToEnumerableAsync();
            foreach (var name in list)
            {
                if (name.Name == namespaceName)
                {
                    isContainnamespaceName = true;
                }
            }
           foreach (var name in list)
            {
                if (name.Id.Contains(resourceGroup))
                {
                    isContainresourceGroup = true;
                    break;
                }
            }
            Assert.True(isContainnamespaceName);
            Assert.True(isContainresourceGroup);
            // Get all namespaces created within the subscription irrespective of the resourceGroup
            var getAllNpResponse = NamespacesClient.ListAsync();
            Assert.NotNull(getAllNamespacesResponse);
            // Update namespace tags and make the namespace critical
            var updateNamespaceParameter = new EHNamespace()
            {
                Tags = new Dictionary<string, string>()
                        {
                            {"tag3", "value3"},
                            {"tag4", "value4"}
                        }
            };
            // Will uncomment the assertions once the service is deployed
            var updateNamespaceResponse = NamespacesClient.UpdateAsync(resourceGroup, namespaceName, updateNamespaceParameter);
            Assert.NotNull(updateNamespaceResponse);
            // Get the updated namespace and also verify the Tags.
            getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            IsDelay(15);
            Assert.NotNull(getNamespaceResponse);
            Assert.AreEqual(Location, getNamespaceResponse.Value.Location);
            Assert.AreEqual(namespaceName, getNamespaceResponse.Value.Name);
            Assert.AreEqual(2, getNamespaceResponse.Value.Tags.Count);
            bool IsContainKey = false;
            bool IsContainValue = false;
            foreach (var tag in updateNamespaceParameter.Tags)
            {
                foreach (var t in getNamespaceResponse.Value.Tags)
                {
                    if (t.Key == tag.Key)
                    {
                        IsContainKey = true;
                        break;
                    }
                }
                foreach (var t in getNamespaceResponse.Value.Tags)
                {
                    if (t.Value == tag.Value)
                    {
                        IsContainValue = true;
                        break;
                    }
                }
            }
            Assert.True(IsContainKey);
            Assert.True(IsContainValue);
            //delete namespace
            await WaitForCompletionAsync(await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName));
        }
    }
}
