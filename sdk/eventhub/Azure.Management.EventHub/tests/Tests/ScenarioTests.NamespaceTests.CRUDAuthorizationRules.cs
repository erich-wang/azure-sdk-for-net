// Copyright (c) Microsoft Corporation. All rights reserved.
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
using System.Collections;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

    public partial class ScenarioTests : EventHubManagementClientBase
    {

        [Test]
        public async Task NamespaceCreateGetUpdateDeleteAuthorizationRules()
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
                    Location = location.Result,
                    //Sku = new Sku("as")
                    Tags = new Dictionary<string, string>()
                        {
                            {"tag1", "value1"},
                            {"tag2", "value2"}
                        }
                }
                );
            var np = (await createNamespaceResponse.WaitForCompletionAsync()).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np.Name, namespaceName);
            isDelay(5);

            //get the created namespace
            var getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            if (string.Compare(getNamespaceResponse.Value.ProvisioningState, "Succeeded", true) != 0)
                isDelay(5);

            getNamespaceResponse = await NamespacesClient.GetAsync(resourceGroup, namespaceName);
            Assert.NotNull(getNamespaceResponse);
            Assert.AreEqual("Succeeded", getNamespaceResponse.Value.ProvisioningState,StringComparer.CurrentCultureIgnoreCase.ToString());
            Assert.AreEqual(location.Result, getNamespaceResponse.Value.Location);

            // Create a namespace AuthorizationRule
            var authorizationRuleName = Recording.GenerateAssetName(Helper.AuthorizationRulesPrefix);
            string createPrimaryKey = Recording.GetVariable("authorizaRuNa", Helper.GenerateRandomKey());
            var createAutorizationRuleParameter = new AuthorizationRule()
            {
                Rights = new List<AccessRights>() { AccessRights.Listen, AccessRights.Send }
            };

            var jsonStr = Helper.ConvertObjectToJSon(createAutorizationRuleParameter);

            var createNamespaceAuthorizationRuleResponse = await NamespacesClient.CreateOrUpdateAuthorizationRuleAsync(resourceGroup, namespaceName,
                authorizationRuleName, createAutorizationRuleParameter);
            Assert.NotNull(createNamespaceAuthorizationRuleResponse);
            Assert.True(createNamespaceAuthorizationRuleResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);
            Assert.True( isContains(createAutorizationRuleParameter.Rights, createNamespaceAuthorizationRuleResponse.Value.Rights));

            // Get default namespace AuthorizationRules
            var getNamespaceAuthorizationRulesResponse = await NamespacesClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, Helper.DefaultNamespaceAuthorizationRule);
            Assert.NotNull(getNamespaceAuthorizationRulesResponse);
            Assert.AreEqual(getNamespaceAuthorizationRulesResponse.Value.Name, Helper.DefaultNamespaceAuthorizationRule);
            var accessRights = new List<AccessRights>() { AccessRights.Listen, AccessRights.Send, AccessRights.Manage};
            Assert.True(isContains(getNamespaceAuthorizationRulesResponse.Value.Rights, accessRights));

            //Collection<AccessRights> listen = new Collection<AccessRights> { AccessRights.Listen };
            //Collection<AccessRights> manage = new Collection<AccessRights> { AccessRights.Manage };
            //Collection<AccessRights> send = new Collection<AccessRights> { AccessRights.Send };
            //Collection<AccessRights> listen = new Collection<AccessRights>{AccessRights.Listen};

            // Get created namespace AuthorizationRules
            getNamespaceAuthorizationRulesResponse =await NamespacesClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, authorizationRuleName);
            Assert.NotNull(getNamespaceAuthorizationRulesResponse);
            Assert.True(getNamespaceAuthorizationRulesResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);
            Assert.True(isContains(createAutorizationRuleParameter.Rights, getNamespaceAuthorizationRulesResponse.Value.Rights));
            //foreach (var right in createAutorizationRuleParameter.Rights)
            //{
            //    Assert.Contains(getNamespaceAuthorizationRulesResponse.Rights, r => r == right);
            //}

            // Get all namespaces AuthorizationRules
            var getAllNamespaceAuthorizationRulesResponse = NamespacesClient.ListAuthorizationRulesAsync(resourceGroup, namespaceName);
            Assert.NotNull(getAllNamespaceAuthorizationRulesResponse);
            var getAllNpsAuthResp = await getAllNamespaceAuthorizationRulesResponse.ToEnumerableAsync();
            Assert.True(getAllNpsAuthResp.Count() > 1);
            bool isContainAuthorizationRuleName=false;
            bool isContainEventHubManagementHelper=false;
            foreach (var Authrule in getAllNpsAuthResp)
            {
                if (Authrule.Name == authorizationRuleName)
                {
                    isContainAuthorizationRuleName = true;
                    break;
                }
            }
            foreach (var Authrule in getAllNpsAuthResp)
            {
                if (Authrule.Name == Helper.DefaultNamespaceAuthorizationRule)
                {
                    isContainEventHubManagementHelper = true;
                    break;
                }
            }
            Assert.True(isContainAuthorizationRuleName);
            Assert.True(isContainEventHubManagementHelper);
            //Assert.Contains(getAllNamespaceAuthorizationRulesResponse, ns => ns.Name == authorizationRuleName);
            //Assert.Contains(getAllNamespaceAuthorizationRulesResponse, auth => auth.Name == EventHubManagementHelper.DefaultNamespaceAuthorizationRule);

            // Update namespace authorizationRule
            string updatePrimaryKey = Recording.GetVariable("UpdatePrimaryKey", Helper.GenerateRandomKey());
            AuthorizationRule updateNamespaceAuthorizationRuleParameter = new AuthorizationRule();
            updateNamespaceAuthorizationRuleParameter.Rights = new List<AccessRights>() { AccessRights.Listen };

            var updateNamespaceAuthorizationRuleResponse =await NamespacesClient.CreateOrUpdateAuthorizationRuleAsync(resourceGroup,
                namespaceName, authorizationRuleName, updateNamespaceAuthorizationRuleParameter);

            Assert.NotNull(updateNamespaceAuthorizationRuleResponse);
            Assert.AreEqual(authorizationRuleName, updateNamespaceAuthorizationRuleResponse.Value.Name);
            Assert.True(updateNamespaceAuthorizationRuleResponse.Value.Rights.Count == updateNamespaceAuthorizationRuleParameter.Rights.Count);

            Assert.True(isContains(updateNamespaceAuthorizationRuleParameter.Rights, updateNamespaceAuthorizationRuleResponse.Value.Rights));
            //foreach (var right in updateNamespaceAuthorizationRuleParameter.Rights)
            //{
            //    Assert.Contains(updateNamespaceAuthorizationRuleResponse.Result.Value.Rights, r => r == right);
            //}

            // Get the updated namespace AuthorizationRule
            var getNamespaceAuthorizationRuleResponse = await NamespacesClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName,
                authorizationRuleName);
            Assert.NotNull(getNamespaceAuthorizationRuleResponse);
            Assert.AreEqual(authorizationRuleName, getNamespaceAuthorizationRuleResponse.Value.Name);
            Assert.True(getNamespaceAuthorizationRuleResponse.Value.Rights.Count == updateNamespaceAuthorizationRuleParameter.Rights.Count);

            Assert.True(isContains(updateNamespaceAuthorizationRuleParameter.Rights, getNamespaceAuthorizationRuleResponse.Value.Rights));
            //foreach (var right in updateNamespaceAuthorizationRuleParameter.Rights)
            //{
            //    Assert.Contains(getNamespaceAuthorizationRuleResponse.Result.Value.Rights, r => r.Equals(right));
            //}

            // Get the connection string to the namespace for a Authorization rule created
            var listKeysResponse =await NamespacesClient.ListKeysAsync(resourceGroup, namespaceName, authorizationRuleName);
            Assert.NotNull(listKeysResponse);
            Assert.NotNull(listKeysResponse.Value.PrimaryConnectionString);
            Assert.NotNull(listKeysResponse.Value.SecondaryConnectionString);

            // Regenerate connection string to the namespace for a Authorization rule created
            var NewKeysResponse_primary = await NamespacesClient.RegenerateKeysAsync(resourceGroup, namespaceName, authorizationRuleName, new RegenerateAccessKeyParameters(KeyType.PrimaryKey));
            Assert.NotNull(NewKeysResponse_primary);
            Assert.AreNotEqual(NewKeysResponse_primary.Value.PrimaryConnectionString, listKeysResponse.Value.PrimaryConnectionString);
            Assert.AreEqual(NewKeysResponse_primary.Value.SecondaryConnectionString, listKeysResponse.Value.SecondaryConnectionString);

            // Regenerate connection string to the namespace for a Authorization rule created
            var NewKeysResponse_secondary =await NamespacesClient.RegenerateKeysAsync(resourceGroup, namespaceName, authorizationRuleName, new RegenerateAccessKeyParameters(KeyType.SecondaryKey));
            Assert.NotNull(NewKeysResponse_secondary);
            Assert.AreNotEqual(NewKeysResponse_secondary.Value.PrimaryConnectionString, listKeysResponse.Value.PrimaryConnectionString);
            Assert.AreNotEqual(NewKeysResponse_secondary.Value.SecondaryConnectionString, listKeysResponse.Value.SecondaryConnectionString);

            // Delete namespace authorizationRule
            await NamespacesClient.DeleteAuthorizationRuleAsync(resourceGroup, namespaceName, authorizationRuleName);

            isDelay(5);

            // Delete namespace
            await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName);

        }
    }
}
