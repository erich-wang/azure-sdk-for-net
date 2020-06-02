﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Azure.Management.EventHub.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Management.EventHub.Models;
    using NUnit.Framework;
    public partial class ScenarioTests : EventHubManagementClientBase
    {
        [Test]
        public async Task DisasterRecoveryAlertnateNameCreateGetUpdateDelete()
        {
            var location = "South Central US";
            var location2 = "North Central US";
            var resourceGroup = Recording.GenerateAssetName(Helper.ResourceGroupPrefix);
            await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient,location, resourceGroup);
            var namespaceName = Recording.GenerateAssetName(Helper.NamespacePrefix);
            // Create namespace 1
            var createNamespaceResponse = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName,
                new EHNamespace()
                {
                    Location = location,
                    Sku = new Sku(SkuName.Standard)
                    {
                        Tier = SkuTier.Standard,
                        Capacity = 1
                    },
                    Tags = new Dictionary<string, string>()
                        {
                            {"tag1", "value1"},
                            {"tag2", "value2"}
                        }
                }
                );
            var np1 = (await WaitForCompletionAsync(createNamespaceResponse)).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np1.Name, namespaceName);
            IsDelay(5);
            // Create namespace 2
            var namespaceName2 = Recording.GenerateAssetName(Helper.NamespacePrefix);
            var createNamespaceResponse2 = await NamespacesClient.StartCreateOrUpdateAsync(resourceGroup, namespaceName2,
               new EHNamespace()
               {
                   Location = location2,
                   Sku = new Sku(SkuName.Standard)
                   {
                       Tier = SkuTier.Standard,
                       Capacity = 1
                   },
                   Tags = new Dictionary<string, string>()
                       {
                            {"tag1", "value1"},
                            {"tag2", "value2"}
                       }
               }
               );
            var np2 = (await WaitForCompletionAsync (createNamespaceResponse2)).Value;
            Assert.NotNull(createNamespaceResponse);
            Assert.AreEqual(np2.Name, namespaceName2);
            IsDelay(5);
            // Create a namespace AuthorizationRule
            var authorizationRuleName = Recording.GenerateAssetName(Helper.AuthorizationRulesPrefix);
            var createAutorizationRuleParameter = new AuthorizationRule()
            {
                Rights = new List<AccessRights>() { AccessRights.Listen,AccessRights.Send}
            };
            var createNamespaceAuthorizationRuleResponse =await NamespacesClient.CreateOrUpdateAuthorizationRuleAsync(resourceGroup, namespaceName,
                authorizationRuleName, createAutorizationRuleParameter);
            Assert.NotNull(createNamespaceAuthorizationRuleResponse);
            Assert.True(createNamespaceAuthorizationRuleResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);
            Assert.True(isContains(createAutorizationRuleParameter.Rights, createNamespaceAuthorizationRuleResponse.Value.Rights));
            // Get created namespace AuthorizationRules
            var getNamespaceAuthorizationRulesResponse = await NamespacesClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, authorizationRuleName);
            Assert.NotNull(getNamespaceAuthorizationRulesResponse);
            Assert.True(getNamespaceAuthorizationRulesResponse.Value.Rights.Count == createAutorizationRuleParameter.Rights.Count);
            Assert.True(isContains(createAutorizationRuleParameter.Rights, getNamespaceAuthorizationRulesResponse.Value.Rights));
            var getNamespaceAuthorizationRulesListKeysResponse = NamespacesClient.ListKeysAsync(resourceGroup, namespaceName, authorizationRuleName);
            // Create a Disaster Recovery -
            var alternateName = Recording.GenerateAssetName(Helper.DisasterRecoveryPrefix);
            //CheckNameavaliability for Alias
            var checknameAlias =await DisasterRecoveryConfigsClient.CheckNameAvailabilityAsync(resourceGroup, namespaceName, new CheckNameAvailabilityParameter(namespaceName));
            Assert.True(checknameAlias.Value.NameAvailable, "The Alias Name: '" + namespaceName + "' is not avilable");
            var DisasterRecoveryResponse =await DisasterRecoveryConfigsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, namespaceName, new ArmDisasterRecovery()
            {
                PartnerNamespace = np2.Id,
                AlternateName = alternateName
            });
            Assert.NotNull(DisasterRecoveryResponse);
            IsDelay(30);
            //// Get the created DisasterRecovery config - Primary
            var disasterRecoveryGetResponse = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName);
            Assert.NotNull(disasterRecoveryGetResponse);
            Assert.AreEqual(RoleDisasterRecovery.Primary, disasterRecoveryGetResponse.Value.Role);
            //// Get the created DisasterRecovery config - Secondary
            var disasterRecoveryGetResponse_Sec =await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName2, namespaceName);
            Assert.NotNull(disasterRecoveryGetResponse_Sec);
            Assert.AreEqual(RoleDisasterRecovery.Secondary, disasterRecoveryGetResponse_Sec.Value.Role);
            //Get authorization rule thorugh Alias
            var getAuthoRuleAliasResponse = await DisasterRecoveryConfigsClient.GetAuthorizationRuleAsync(resourceGroup, namespaceName, namespaceName, authorizationRuleName);
            Assert.AreEqual(getAuthoRuleAliasResponse.Value.Name, getNamespaceAuthorizationRulesResponse.Value.Name);
            var getAuthoruleListKeysResponse = await DisasterRecoveryConfigsClient.ListKeysAsync(resourceGroup, namespaceName, namespaceName, authorizationRuleName);
            var disasterRecoveryGetResponse_Accepted =await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName);
            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName).Result.Value.ProvisioningState!= ProvisioningStateDR.Succeeded)
            {
                IsDelay(10);
            }
            //// Break Pairing
            await DisasterRecoveryConfigsClient.BreakPairingAsync(resourceGroup, namespaceName, namespaceName);
            IsDelay(10);
            disasterRecoveryGetResponse_Accepted = await DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName);
            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                IsDelay(10);
            }
            var DisasterRecoveryResponse_update = DisasterRecoveryConfigsClient.CreateOrUpdateAsync(resourceGroup, namespaceName, namespaceName, new ArmDisasterRecovery()
            {
                PartnerNamespace = np2.Id,
                AlternateName = alternateName
            });
            Assert.NotNull(DisasterRecoveryResponse_update);
            IsDelay(10);
            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName, namespaceName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                IsDelay(10);
            }
            // Fail over
            await DisasterRecoveryConfigsClient.FailOverAsync(resourceGroup, namespaceName2, namespaceName);
            IsDelay(10);
            while (DisasterRecoveryConfigsClient.GetAsync(resourceGroup, namespaceName2, namespaceName).Result.Value.ProvisioningState != ProvisioningStateDR.Succeeded)
            {
                IsDelay(10);
            }
            // Get all Disaster Recovery for a given NameSpace
            var getListisasterRecoveryResponse = DisasterRecoveryConfigsClient.ListAsync(resourceGroup, namespaceName2);
            Assert.NotNull(getListisasterRecoveryResponse);
            //Assert.True(getListisasterRecoveryResponse.AsPages.Count<ArmDisasterRecovery>() >= 1);
            // Delete the DisasterRecovery
            await DisasterRecoveryConfigsClient.DeleteAsync(resourceGroup, namespaceName2, namespaceName);
            // Delete Namespace using Async
            //await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName, new CancellationToken()).ConfigureAwait(false);
            ////NamespacesClient.DeleteWithHttpMessages(resourceGroup, namespaceName, null, new CancellationToken()).ConfigureAwait(false);
            //await NamespacesClient.StartDeleteAsync(resourceGroup, namespaceName2, new CancellationToken()).ConfigureAwait(false);
        }
        public bool isContains(IList<AccessRights> accessRights, IList<AccessRights> accessRightsComp)
        {
            bool isAllContain = true;
            foreach (var rightDetail in accessRights)
            {
                bool isContain = false;
                foreach (var compareRight in accessRightsComp)
                {
                    if (compareRight == rightDetail)
                    {
                        isContain = true;
                        break;
                    }
                }
                if (isContain == false)
                {
                    isAllContain = false;
                }
            }
            return isAllContain;
        }
    }
}
