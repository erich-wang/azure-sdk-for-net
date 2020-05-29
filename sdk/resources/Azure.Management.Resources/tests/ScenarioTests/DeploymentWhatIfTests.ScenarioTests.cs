using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources.Tests;
using Azure.Management.Resources.Models;
using System.IO;
using Azure.Management.Resources;
using NUnit.Framework;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using System.Reflection;
using System.Text.Json;
using Azure.Core;

namespace ResourceGroups.Tests
{
    public class LiveDeploymentWhatIfTests : ResourceOperationsTestsBase
    {
        public LiveDeploymentWhatIfTests(bool isAsync)
            : base(isAsync)
        {
        }

        private static readonly ResourceGroup ResourceGroup = new ResourceGroup("westus");

        private static readonly string BlankTemplate = LoadTemplateContent("blank_template.json");

        private static readonly string ResourceGroupTemplate = LoadTemplateContent("simple-storage-account.json");

        private static readonly string ResourceGroupTemplateGRS = LoadTemplateContent("simple-storage-account-GRS.json");

        private static readonly string ResourceGroupTemplateParameters = LoadTemplateContent("simple-storage-account-parameters.json");

        private static readonly string SubscriptionTemplate = LoadTemplateContent("subscription_level_template.json");

        private static readonly string SubscriptionTemplateWestEurope = LoadTemplateContent("subscription_level_template_westeurope.json");
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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(BlankTemplate);
            object template = jsonTemplate.GetObject();
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                    {
                        Template = template,
                        WhatIfSettings = new DeploymentWhatIfSettings()
                        {
                            ResultFormat = WhatIfResultFormat.FullResourcePayloads
                        }
                    }
                );

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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplate);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplateParameters);
            object template = jsonTemplate.GetObject();
            var parameterDictionary = jsonParameter.GetObject() as IDictionary<string, object>;
            object parameter = parameterDictionary;
            if (parameterDictionary.ContainsKey("parameters"))
            {
                parameter = parameterDictionary["parameters"];
            }

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = parameter,
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.ResourceIdOnly
                    }
                }
                );

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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplate);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplateParameters);
            object template = jsonTemplate.GetObject();
            var parameterDictionary = jsonParameter.GetObject() as IDictionary<string, object>;
            object parameter = parameterDictionary;
            if (parameterDictionary.ContainsKey("parameters"))
            {
                parameter = parameterDictionary["parameters"];
            }

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                    {
                        Template = template,
                        Parameters = parameter,
                        WhatIfSettings = new DeploymentWhatIfSettings()
                        {
                            ResultFormat = WhatIfResultFormat.ResourceIdOnly
                        }
                    }
                );

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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplate);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplateParameters);
            object template = jsonTemplate.GetObject();
            var parameterDictionary = jsonParameter.GetObject() as IDictionary<string, object>;
            object parameter = parameterDictionary;
            if (parameterDictionary.ContainsKey("parameters"))
            {
                parameter = parameterDictionary["parameters"];
            }

            var deployment = new Deployment(new DeploymentProperties(DeploymentMode.Incremental)
            {
                Template = template,
                Parameters = parameter
            });

            // Modify account type: Standard_LRS => Standard_GRS.
            JsonElement newTemplate = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplateGRS);
            template = newTemplate.GetObject();

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = parameter,
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.FullResourcePayloads
                    }
                }
                );

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
                change.ResourceId.EndsWith("Microsoft.Storage/storageAccounts/ramokaSATestAnother1"));

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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplate);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(ResourceGroupTemplateParameters);
            JsonElement jsonBlankTemplate = JsonSerializer.Deserialize<JsonElement>(BlankTemplate);
            object template = jsonTemplate.GetObject();
            object blankTemplate = jsonBlankTemplate.GetObject();
            var parameterDictionary = jsonParameter.GetObject() as IDictionary<string, object>;
            object parameter = parameterDictionary;
            if (parameterDictionary.ContainsKey("parameters"))
            {
                parameter = parameterDictionary["parameters"];
            }

            var deployment = new Deployment(new DeploymentProperties(DeploymentMode.Incremental)
            {
                Template = template,
                Parameters = parameter
            });
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Complete)
                {
                    Template = blankTemplate,
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.ResourceIdOnly
                    }
                }
                );

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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(BlankTemplate);
            object template = jsonTemplate.GetObject();

            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.ResourceIdOnly
                    }
                }
                )
            { Location = "westus" };
            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf);
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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(SubscriptionTemplate);
            object template = jsonTemplate.GetObject();
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.ResourceIdOnly
                    }
                }
                )
            { Location = "westus" };

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf);
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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(SubscriptionTemplate);
            object template = jsonTemplate.GetObject();
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "whatifnetsdktest1" }
                                        }
                                    }
                                },
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.ResourceIdOnly
                    }
                }
                )
            { Location = "westus2" };

            // Use resource group name from the template.
            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync("SDK-test", ResourceGroup)).Value;

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf);
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
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(SubscriptionTemplate);
            object template = jsonTemplate.GetObject();
            var deployment = new Deployment(
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "whatifnetsdktest1" }
                                        }
                                    }
                                }
                }) {
                Location = "westus2"
            };

            // Change "northeurope" to "westeurope".
            JsonElement newJsonTemplate = JsonSerializer.Deserialize<JsonElement>(SubscriptionTemplateWestEurope);
            object newTemplate = newJsonTemplate.GetObject();

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = newTemplate,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "whatifnetsdktest1" }
                                        }
                                    }
                                },
                    WhatIfSettings = new DeploymentWhatIfSettings()
                    {
                        ResultFormat = WhatIfResultFormat.FullResourcePayloads
                    }
                }
                )
            { Location = "westus2" };

            var resourcegroup = (await ResourceGroupsClient.CreateOrUpdateAsync("SDK-test", ResourceGroup)).Value;
            var deploy = await DeploymentsClient.StartCreateOrUpdateAtSubscriptionScopeAsync(NewDeploymentName(), deployment);
            await deploy.WaitForCompletionAsync();

            // Act.
            var rawResult = await DeploymentsClient.StartWhatIfAtSubscriptionScopeAsync(NewDeploymentName(), deploymentWhatIf);
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
