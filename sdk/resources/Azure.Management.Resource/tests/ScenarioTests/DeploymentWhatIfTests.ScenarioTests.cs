using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resource.Tests;
using Azure.Management.Resource.Models;
using System.IO;
using Azure.Management.Resource;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Azure.Core.Testing;
using System.Reflection;

namespace ResourceGroups.Tests
{
    public class LiveDeploymentWhatIfTests : ResourceOperationsTestsBase
    {
        public LiveDeploymentWhatIfTests(bool isAsync, ResourceManagementClientOptions.ServiceVersion serviceVersion)
            : base(isAsync, serviceVersion)
        {
        }

        private static readonly ResourceGroup ResourceGroup = new ResourceGroup("westus");

        private static readonly string BlankTemplate = LoadTemplateContent("blank_template.json");

        private static readonly string ResourceGroupTemplate = LoadTemplateContent("simple-storage-account.json");

        private static readonly string ResourceGroupTemplateParameters = LoadTemplateContent("simple-storage-account-parameters.json");

        private static readonly string SubscriptionTemplate = LoadTemplateContent("subscription_level_template.json");

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        public async Task WhatIf_BlankTemplate_ReturnsNoChange()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = BlankTemplate,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.FullResourcePayloads
                }
            };

            string resourceGroupName = NewResourceGroupName();
            string deploymentName = NewDeploymentName();

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, ResourceGroup)).Value;

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAsync(resourceGroupName, deploymentName, deploymentWhatIf);
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsEmpty(result.Changes);
        }

        [Test]
        public async Task WhatIf_ResourceIdOnlyMode_ReturnsChangesWithResourceIdsOnly()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = ResourceGroupTemplate,
                Parameters = ResourceGroupTemplateParameters,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };

            string resourceGroupName = NewResourceGroupName();
            string deploymentName = NewDeploymentName();

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, ResourceGroup)).Value;

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAsync(resourceGroupName, deploymentName, deploymentWhatIf);
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);

            foreach (var change in result.Changes)
            {
                Assert.NotNull(change.ResourceId);
                Assert.IsNotEmpty(change.ResourceId);
                Assert.AreEqual(ChangeType.Create, change.ChangeType);
                Assert.Null(change.Before);
                Assert.Null(change.After);
                Assert.Null(change.Delta);
            }
        }
        [Test]
        public async Task WhatIf_CreateResources_ReturnsCreateChanges()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = ResourceGroupTemplate,
                Parameters = ResourceGroupTemplateParameters,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };

            string resourceGroupName = NewResourceGroupName();
            string deploymentName = NewDeploymentName();

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, ResourceGroup)).Value;

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAsync(resourceGroupName, deploymentName, deploymentWhatIf);
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);
            foreach (var change in result.Changes)
            {
                Assert.AreEqual(ChangeType.Create, change.ChangeType);
            }
        }

        [Test]
        public async Task WhatIf_ModifyResources_ReturnsModifyChanges()
        {
            // Arrange.
            var deployment = new Deployment(new DeploymentProperties(DeploymentMode.Incremental)
            {
                Template = ResourceGroupTemplate,
                Parameters = ResourceGroupTemplateParameters
            });

            // Modify account type: Standard_LRS => Standard_GRS.
            JObject newTemplate = JObject.Parse(ResourceGroupTemplate);
            newTemplate["resources"][0]["properties"]["accountType"] = "Standard_GRS";

            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = newTemplate,
                Parameters = ResourceGroupTemplateParameters,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.FullResourcePayloads
                }
            };

            string resourceGroupName = NewResourceGroupName();
            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, ResourceGroup)).Value;
            var deploy = await DeploymentsClient.StartCreateOrUpdateAsync(resourceGroupName, NewDeploymentName(), deployment);
            await deploy.WaitForCompletionAsync();

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAsync(resourceGroupName, NewDeploymentName(), deploymentWhatIf);
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);

            WhatIfChange storageAccountChange = result.Changes.FirstOrDefault(change =>
                change.ResourceId.EndsWith("Microsoft.Storage/storageAccounts/ramokaSATestAnother"));

            Assert.NotNull(storageAccountChange);
            Assert.AreEqual(ChangeType.Modify, storageAccountChange.ChangeType);

            Assert.NotNull(storageAccountChange.Delta);
            Assert.IsNotEmpty(storageAccountChange.Delta);

            WhatIfPropertyChange accountTypeChange = storageAccountChange.Delta
                .FirstOrDefault(propertyChange => propertyChange.Path.Equals("properties.accountType"));

            Assert.NotNull(accountTypeChange);
            Assert.AreEqual("Standard_LRS", accountTypeChange.Before);
            Assert.AreEqual("Standard_GRS", accountTypeChange.After);
        }

        [Test]
        public async Task WhatIf_DeleteResources_ReturnsDeleteChanges()
        {
            // Arrange.
            var deployment = new Deployment(new DeploymentProperties(DeploymentMode.Incremental)
            {
                Template = ResourceGroupTemplate,
                Parameters = ResourceGroupTemplateParameters
            });
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Complete)
            {
                Template = BlankTemplate,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };

            string resourceGroupName = NewResourceGroupName();

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, ResourceGroup)).Value;
            var deploy = await DeploymentsClient.StartCreateOrUpdateAsync(resourceGroupName, NewDeploymentName(), deployment);
            await deploy.WaitForCompletionAsync();

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAsync(resourceGroupName, NewDeploymentName(), deploymentWhatIf);
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);
            foreach (var change in result.Changes)
            {
                Assert.AreEqual(ChangeType.Delete, change.ChangeType);
            }
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_BlankTemplate_ReturnsNoChange()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = BlankTemplate,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };
            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf, "westus");
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsEmpty(result.Changes);
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_ResourceIdOnlyMode_ReturnsChangesWithResourceIdsOnly()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = SubscriptionTemplate,
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf, "westus");
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);
            foreach (var change in result.Changes)
            {
                Assert.NotNull(change.ResourceId);
                Assert.IsNotEmpty(change.ResourceId);
                Assert.True(change.ChangeType == ChangeType.Deploy || change.ChangeType == ChangeType.Create);
                Assert.Null(change.Before);
                Assert.Null(change.After);
                Assert.Null(change.Delta);
            }
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_CreateResources_ReturnsCreateChanges()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Incremental)
            {
                Template = SubscriptionTemplate,
                Parameters = JObject.Parse("{ 'storageAccountName': {'value': 'whatifnetsdktest1'}}"),
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.ResourceIdOnly
                }
            };

            // Use resource group name from the template.
            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync("SDK-test", ResourceGroup)).Value;

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf, "westus2");
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);
            foreach (var change in result.Changes)
            {
                if (change.ResourceId.EndsWith("SDK-test"))
                {
                    Assert.AreEqual(ChangeType.Ignore, change.ChangeType);
                }
                else
                {
                    Assert.True(change.ChangeType == ChangeType.Deploy || change.ChangeType == ChangeType.Create);
                }
            }
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_ModifyResources_ReturnsModifyChanges()
        {
            // Arrange.
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = SubscriptionTemplate,
                    Parameters = JObject.Parse("{ 'storageAccountName': {'value': 'whatifnetsdktest1'}}")
                }) {
                Location = "westus2"
            };

            // Change "northeurope" to "westeurope".
            JObject newTemplate = JObject.Parse(SubscriptionTemplate);
            newTemplate["resources"][0]["properties"]["policyRule"]["if"]["equals"] = "westeurope";

            var deploymentWhatIf = new DeploymentWhatIfProperties(DeploymentMode.Complete)
            {
                Template = newTemplate,
                Parameters = JObject.Parse("{ 'storageAccountName': {'value': 'whatifnetsdktest1'}}"),
                WhatIfSettings = new DeploymentWhatIfSettings()
                {
                    ResultFormat = WhatIfResultFormat.FullResourcePayloads
                }
            };

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync("SDK-test", ResourceGroup)).Value;
            var deploy = await DeploymentsClient.StartCreateOrUpdateAtSubscriptionScopeAsync(NewDeploymentName(), deployment);
            await deploy.WaitForCompletionAsync();

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf, "westus2");
            var result = (await rawResult.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.IsNotEmpty(result.Changes);

            WhatIfChange policyChange = result.Changes.FirstOrDefault(change =>
                change.ResourceId.EndsWith("Microsoft.Authorization/policyDefinitions/policy2"));

            Assert.NotNull(policyChange);
            Assert.True(policyChange.ChangeType == ChangeType.Deploy ||
                        policyChange.ChangeType == ChangeType.Modify);
            Assert.NotNull(policyChange.Delta);
            Assert.IsNotEmpty(policyChange.Delta);

            WhatIfPropertyChange policyRuleChange = policyChange.Delta
                .FirstOrDefault(propertyChange => propertyChange.Path.Equals("properties.policyRule.if.equals"));

            Assert.NotNull(policyRuleChange);
            Assert.AreEqual(PropertyChangeType.Modify, policyRuleChange.PropertyChangeType);
            Assert.AreEqual("northeurope", policyRuleChange.Before);
            Assert.AreEqual("westeurope", policyRuleChange.After);
        }

        private static string LoadTemplateContent(string filePath)
            => File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", filePath));

        private string NewResourceGroupName() => Recording.GenerateAssetName("csmd");

        private string NewDeploymentName() => Recording.GenerateAssetName("csmd");
    }
}
