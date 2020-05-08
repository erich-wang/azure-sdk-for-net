// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.EventHub.Models;
using NUnit.Framework;


    public partial class ScenarioTests : EventHubManagementClientBase
    {

        [Test]
        public async Task DisasterRecoveryCreateGetUpdateDelete()
        {
            var location = "South Central US";
            var location2 = "North Central US";
            var resourceGroup = TryGetResourceGroup(ResourceGroupsClient, location);
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
                await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient,location, resourceGroup);
            }

            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);

            // Create namespace 1
            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location,
                    //Sku = new Sku
                    //{
                    //    Name = SkuName.Standard,
                    //    Tier = SkuTier.Standard,
                    //    Capacity = 1
                    //},
                    Tags = new Dictionary<string, string>()
                        {
                            {"tag1", "value1"},
                            {"tag2", "value2"}
                        }
                }
                );
            var np1 = (await createNamespaceResponse.WaitForCompletionAsync()).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np1.Name, namespaceName);
            isDelay(5);

            // Create namespace 2
            var namespaceName2 = Recording.GenerateAssetName(Helper.NamespacePrefix);
            var createNamespaceResponse2 = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName2,
               new EHNamespace()
               {
                   Location = location2,
                   //Sku = new Sku
                   //{
                   //    Name = SkuName.Standard,
                   //    Tier = SkuTier.Standard,
                   //    Capacity = 1
                   //},
                   Tags = new Dictionary<string, string>()
                       {
                            {"tag1", "value1"},
                            {"tag2", "value2"}
                       }
               }
               );
            var np2 = (await createNamespaceResponse2.WaitForCompletionAsync()).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np2.Name, namespaceName2);
            isDelay(5);

            // Create a namespace AuthorizationRule
            var authorizationRuleName = Recording.GenerateAssetName(Helper.AuthorizationRulesPrefix);
            var createAutorizationRuleParameter = new AuthorizationRule()
            {
                Rights = new List<AccessRights>() { AccessRights.Listen, AccessRights.Send }
            };

            var createNamespaceAuthorizationRuleResponse = await NamespacesClient.CreateOrUpdateAuthorizationRuleAsync(resourceGroup, namespaceName,
                authorizationRuleName, createAutorizationRuleParameter);
            Assert.NotNull(createNamespaceAuthorizationRuleResponse);
            Assert.True(createNamespaceAuthorizationRuleResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);

            Assert.True(isContains(createAutorizationRuleParameter.Rights, createNamespaceAuthorizationRuleResponse.Value.Rights));
            //foreach (var rightDetail in createAutorizationRuleParameter.Rights)
            //{
            //    bool isContain = false;
            //    foreach (var compareRight in createNamespaceAuthorizationRuleResponse.Value.Rights)
            //    {
            //        if (compareRight == rightDetail)
            //        {
            //            isContain = true;
            //            break;
            //        }
            //    }
            //    Assert.True(isContain);
            //}

            // Get created namespace AuthorizationRules
            var getNamespaceAuthorizationRulesResponse = await NamespacesClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, authorizationRuleName);
            Assert.NotNull(getNamespaceAuthorizationRulesResponse);
            Assert.True(getNamespaceAuthorizationRulesResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);
            //foreach (var right in createAutorizationRuleParameter.Rights)
            //{
            //    Assert.Contains(getNamespaceAuthorizationRulesResponse.Value.Rights, r => r == right);
            //}
            Assert.True(isContains(createAutorizationRuleParameter.Rights, getNamespaceAuthorizationRulesResponse.Value.Rights));
            //Collection<AccessRights> getNamespAuthRulesResp = new Collection<AccessRights>(createAutorizationRuleParameter.Rights);
            //Assert.Contains(getNamespaceAuthorizationRulesResponse.Value.Rights, getNamespAuthRulesResp);

            var getNamespaceAuthorizationRulesListKeysResponse = NamespacesClient.ListKeysAsync(resourceGroup, namespaceName, authorizationRuleName);

            // Create a Disaster Recovery -
            var disasterRecoveryName = Recording.GenerateAssetName(Helper.DisasterRecoveryPrefix);

            //CheckNameavaliability for Alias

            var checknameAlias = await DisasterRecoveryConfigsClient.CheckNameAvailabilityAsync(resourceGroup, namespaceName, new CheckNameAvailabilityParameter(disasterRecoveryName));

            Assert.True(checknameAlias.Value.NameAvailable, "The Alias Name: '" + disasterRecoveryName + "' is not avilable");

            //CheckNameAvaliability for Alias with same as namespace name (alternateName will be used in this case)
            var checknameAliasSame =await DisasterRecoveryConfigsClient.CheckNameAvailabilityAsync(resourceGroup, namespaceName, new CheckNameAvailabilityParameter(namespaceName));

            // Assert.True(checknameAliasSame.NameAvailable, "The Alias Name: '" + namespaceName + "' is not avilable");

            var DisasterRecoveryResponse = await DisasterRecoveryConfigsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, disasterRecoveryName, new ArmDisasterRecovery()
            {
                PartnerNamespace = np2.Id
            });
            Assert.NotNull(DisasterRecoveryResponse);

            isDelay(30);

            //// Get the created DisasterRecovery config - Primary
            var disasterRecoveryGetResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName);
            Assert.NotNull(disasterRecoveryGetResponse);
            if (disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount.HasValue)
                Assert.True(disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount >= 0);
            else
                Assert.False(disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount.HasValue);
            Assert.AreEqual(RoleDisasterRecovery.Primary, disasterRecoveryGetResponse.Value.Role);

            //// Get the created DisasterRecovery config - Secondary
            var disasterRecoveryGetResponse_Sec = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName2, disasterRecoveryName);
            Assert.AreEqual(RoleDisasterRecovery.Secondary, disasterRecoveryGetResponse_Sec.Value.Role);

            //Get authorization rule thorugh Alias

            var getAuthoRuleAliasResponse = await DisasterRecoveryConfigsClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, disasterRecoveryName, authorizationRuleName);
            Assert.AreEqual(getAuthoRuleAliasResponse.Value.Name, getNamespaceAuthorizationRulesResponse.Value.Name);

            var getAuthoruleListKeysResponse = await DisasterRecoveryConfigsClient.ListKeysAsync(resourceGroup, namespaceName, disasterRecoveryName, authorizationRuleName);
            Assert.True(string.IsNullOrEmpty(getAuthoruleListKeysResponse.Value.PrimaryConnectionString));
            Assert.True(string.IsNullOrEmpty(getAuthoruleListKeysResponse.Value.SecondaryConnectionString));

            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                isDelay(10);
            }

            disasterRecoveryGetResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName);

            if (disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount.HasValue)
                Assert.True(disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount >= 0);
            else
                Assert.False(disasterRecoveryGetResponse.Value.PendingReplicationOperationsCount.HasValue);

            //// Break Pairing
            await DisasterRecoveryConfigsClient.BreakPairingAsync(resourceGroup, namespaceName, disasterRecoveryName);
            isDelay(10);

            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                isDelay(10);
            }

            var DisasterRecoveryResponse_update = await DisasterRecoveryConfigsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, disasterRecoveryName, new ArmDisasterRecovery()
            {
                PartnerNamespace = np2.Id
            });

            Assert.NotNull(DisasterRecoveryResponse_update);
            isDelay(10);

            var getGeoDRResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName);

            while (getGeoDRResponse.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                getGeoDRResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName);
                isDelay(10);
            }

            getGeoDRResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, disasterRecoveryName);

            if (getGeoDRResponse.Value.PendingReplicationOperationsCount.HasValue)
                Assert.True(getGeoDRResponse.Value.PendingReplicationOperationsCount >= 0);
            else
                Assert.False(getGeoDRResponse.Value.PendingReplicationOperationsCount.HasValue);

            // Fail over
            await DisasterRecoveryConfigsClient.FailOverAsync(resourceGroup, namespaceName2, disasterRecoveryName);

            isDelay(10);

            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName2, disasterRecoveryName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                isDelay(10);
            }

            // Get all Disaster Recovery for a given NameSpace
            var getListisasterRecoveryResponse = DisasterRecoveryConfigsClient.ListAsync(resourceGroup, namespaceName2);
            Assert.NotNull(getListisasterRecoveryResponse);
            //Assert.True(getListisasterRecoveryResponse.AsPages.Count<ArmDisasterRecovery>() >= 1);

            // Delete the DisasterRecovery
            await DisasterRecoveryConfigsClient.DeleteAsync(resourceGroup, namespaceName2, disasterRecoveryName);

            // Delete Namespace using Async
            //await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName, new CancellationToken()).ConfigureAwait(false);
            ////NamespacesClient.DeleteWithHttpMessages(resourceGroup, namespaceName, null, new CancellationToken()).ConfigureAwait(false);
            //await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName2, new CancellationToken()).ConfigureAwait(false);
            //EventHubManagementClient.Namespaces.DeleteWithHttpMessagesAsync(resourceGroup, namespaceName, null, new CancellationToken()).ConfigureAwait(false);

            //EventHubManagementClient.Namespaces.DeleteWithHttpMessagesAsync(resourceGroup, namespaceName2, null, new CancellationToken()).ConfigureAwait(false);
        }
    }
}
