// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Identity;
using Azure.Management.EventHub.Models;
using NUnit.Framework;


    public partial class ScenarioTests : EventHubManagementClientBase
    {
        [Test]
        public async Task NamespaceCreateGetUpdateDelete()
        {
            var location = GetLocation();
            var resourceGroup = TryGetResourceGroup(ResourceGroupsClient, location.Result);
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
                await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient, location.Result, resourceGroup);
            }
            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);

            var operationsResponse = OperationsClient.ListAsync();

            var checkNameAvailable = NamespacesClient.CheckNameAvailabilityAsync(new CheckNameAvailabilityParameter(namespaceName));

            //var Location = "eastus";
            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location.Result
                    //Sku = new Sku("as")
                }
                );
            var np = (await createNamespaceResponse.WaitForCompletionAsync()).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np.Name,namespaceName);
            isDelay(60);
            //get the created namespace
            var getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            if (string.Compare(getNamespaceResponse.Value.ProvisioningState, "Succeeded", true) != 0)
                isDelay(10);

            getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            Assert.NotNull(getNamespaceResponse);
            Assert.AreEqual("Succeeded", getNamespaceResponse.Value.ProvisioningState,StringComparer.CurrentCultureIgnoreCase.ToString());
            Assert.AreEqual(location.Result, getNamespaceResponse.Value.Location);

            // Get all namespaces created within a resourceGroup
            var getAllNamespacesResponse =  NamespacesClient.ListByResourceGroupAsync(resourceGroup);
            Assert.NotNull(getAllNamespacesResponse);
            //Collection<>
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
            //Assert.Contains(getAllNamespacesResponse,  => ns.Name == namespaceName);
            //Assert.Contains(getAllNamespacesResponse, ns => ns.Id.Contains(resourceGroup))

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
            Assert.NotNull(getNamespaceResponse);
            //Assert.Equals(Location, getNamespaceResponse.Value.Location, StringComparer.CurrentCultureIgnoreCase);
            Assert.AreEqual(namespaceName, getNamespaceResponse.Value.Name);
            Assert.AreEqual(2, getNamespaceResponse.Value.Tags.Count);
            //foreach (var tag in updateNamespaceParameter.Tags)
            //{
            //    Assert.Contains(getNamespaceResponse.Value.Tags, t => t.Key.Equals(tag.Key));
            //    Assert.Contains(getNamespaceResponse.Value.Tags, t => t.Value.Equals(tag.Value));
            //}

            //delete namespace
            await (await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName)).WaitForCompletionAsync();
        }
    }
}
