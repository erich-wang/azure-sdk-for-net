using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Resources.Tests;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ResourceGroups.Tests
{
    public class InMemoryDeploymentWhatIfTests : ResourceOperationsTestsBase
    {
        public InMemoryDeploymentWhatIfTests(bool isAsync) : base(isAsync)
        {
        }

        public ResourcesManagementClient GetResourceManagementClient(HttpPipelineTransport transport)
        {
            ResourcesManagementClientOptions options = new ResourcesManagementClientOptions();
            options.Transport = transport;

            return InstrumentClient(new ResourcesManagementClient(
                TestEnvironment.SubscriptionId,
                new TestCredential(), options));
        }

        [Test]
        public async Task WhatIf_SendingRequest_SetsHeaders()
        {
            // Arrange.
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var deploymentWhatIf = new DeploymentWhatIf(new DeploymentWhatIfProperties(DeploymentMode.Incremental));

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAsync("test-rg", "test-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Assert.IsTrue(request.Headers.Contains(new HttpHeader("Content-Type", "application/json")));
            Assert.AreEqual(HttpMethod.Post.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task WhatIf_SendingRequest_SerializesPayload()
        {
            // Arrange.
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var deploymentWhatIf = new DeploymentWhatIf(new DeploymentWhatIfProperties(DeploymentMode.Complete)
            {
                TemplateLink = new TemplateLink("https://example.com")
                {
                    ContentVersion = "1.0.0.0"
                },
                ParametersLink = new ParametersLink("https://example.com/parameters")
                {
                    ContentVersion = "1.0.0.0"
                },
                Template = new Dictionary<string, object> { { "$schema", "fake" } },
                Parameters = new Dictionary<string, object>
                {
                    ["param1"] = "foo",
                    ["param2"] = new Dictionary<string, object>
                    {
                        ["param2_1"] = 123,
                        ["param2_2"] = "bar"
                    }
                }
            })
            {
                Location = "westus"
            };

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAsync("test-rg", "test-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var resultJson = JObject.Parse(resquestContent);
            Assert.AreEqual("westus", resultJson["location"].Value<string>());
            Assert.AreEqual("Complete", resultJson["properties"]["mode"].Value<string>());
            Assert.AreEqual("https://example.com", resultJson["properties"]["templateLink"]["uri"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["templateLink"]["contentVersion"].Value<string>());
            Assert.AreEqual("https://example.com/parameters", resultJson["properties"]["parametersLink"]["uri"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["parametersLink"]["contentVersion"].Value<string>());
            Assert.AreEqual(JObject.Parse("{ '$schema': 'fake' }"), resultJson["properties"]["template"]);
            Assert.AreEqual("foo", resultJson["properties"]["parameters"]["param1"].Value<string>());
            Assert.AreEqual(123, resultJson["properties"]["parameters"]["param2"]["param2_1"].Value<int>());
            Assert.AreEqual("bar", resultJson["properties"]["parameters"]["param2"]["param2_2"].Value<string>());
        }

        [Test]
        public async Task WhatIf_SendingRequestWithStrings_SerializesPayload()
        {
            // Arrange.
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);

            const string templateContent = "{" +
                "\"$schema\": \"http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#\"," +
                "\"contentVersion\": \"1.0.0.0\"," +
                "\"parameters\": {" +
                    "\"storageAccountName\": {" +
		                "\"type\": \"string\"" +
	                "}" +
                "}," +
                "\"resources\": [" +
                "]," +
                "\"outputs\": {}" +
            "}";
            const string parameterContent = "{" +
            "\"storageAccountName\": {" +
                "\"value\": \"lsfjlasf9urw\"" +
            "}" +
            "}";
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(templateContent);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(parameterContent);

            object template = jsonTemplate.GetObject();
            object parameter = jsonParameter.GetObject();

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                    {
                        Template = template,
                        Parameters = parameter
                })
            {
                Location = "westus"
            };

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAsync("test-rg", "test-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var resultJson = JObject.Parse(resquestContent);
            Assert.AreEqual("Incremental", resultJson["properties"]["mode"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["template"]["contentVersion"].Value<string>());
            Assert.AreEqual("lsfjlasf9urw", resultJson["properties"]["parameters"]["storageAccountName"]["value"].Value<string>());
        }

        [Test]
        public async Task WhatIf_ReceivingResponse_DeserializesResult()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIf(new DeploymentWhatIfProperties(DeploymentMode.Incremental));
            var content = JObject.Parse(@"{
                    'status': 'Succeeded',
                    'properties': {
                        'changes': [
                            {
                                'resourceId': '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/myResourceGroup/providers/Microsoft.ManagedIdentity/userAssignedIdentities/myExistingIdentity',
                                'changeType': 'Modify',
                                'before': {
                                    'apiVersion': '2018-11-30',
                                    'id': '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/myResourceGroup/providers/Microsoft.ManagedIdentity/userAssignedIdentities/myExistingIdentity',
                                    'type': 'Microsoft.ManagedIdentity/userAssignedIdentities',
                                    'name': 'myExistingIdentity',
                                    'location': 'westus2'
                                },
                                'after': {
                                    'apiVersion': '2018-11-30',
                                    'id': '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/myResourceGroup/providers/Microsoft.ManagedIdentity/userAssignedIdentities/myExistingIdentity',
                                    'type': 'Microsoft.ManagedIdentity/userAssignedIdentities',
                                    'name': 'myExistingIdentity',
                                    'location': 'westus2',
                                    'tags': {
                                        'myNewTag': 'my tag value'
                                    }
                                },
                                'delta': [
                                    {
                                        'path': 'tags.myNewTag',
                                        'propertyChangeType': 'Create',
                                        'after': 'my tag value'
                                    }
                                ]
                            }
                        ]
                    }
                }").ToString();

            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            mockResponse.SetContent(content);
            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            var raw = await client.GetDeploymentsClient().StartWhatIfAsync("test-rg", "test-deploy", deploymentWhatIf);
            var result = (await raw.WaitForCompletionAsync()).Value;

            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.AreEqual(1, result.Changes.Count);

            WhatIfChange change = result.Changes[0];
            Assert.AreEqual(ChangeType.Modify, change.ChangeType);

            Assert.NotNull(change.Before);
            Assert.AreEqual("myExistingIdentity", JToken.FromObject(change.Before)["name"].Value<string>());

            Assert.NotNull(change.After);
            Assert.AreEqual("myExistingIdentity", JToken.FromObject(change.After)["name"].Value<string>());

            Assert.NotNull(change.Delta);
            Assert.AreEqual(1, change.Delta.Count);
            Assert.AreEqual("tags.myNewTag", change.Delta[0].Path);
            Assert.AreEqual(PropertyChangeType.Create, change.Delta[0].PropertyChangeType);
            Assert.AreEqual("my tag value", change.Delta[0].After);
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_SendingRequest_SetsHeaders()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIf(new DeploymentWhatIfProperties(DeploymentMode.Incremental));

            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAtSubscriptionScopeAsync("test-subscription-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Assert.IsTrue(request.Headers.Contains(new HttpHeader("Content-Type", "application/json")));
            Assert.AreEqual(HttpMethod.Post.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_SendingRequest_SerializesPayload()
        {
            // Arrange.
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Complete)
                {
                    TemplateLink = new TemplateLink("https://example.com")
                    {
                        ContentVersion = "1.0.0.0"
                    },
                    ParametersLink = new ParametersLink("https://example.com/parameters")
                    {
                        ContentVersion = "1.0.0.0"
                    },
                    Template = JObject.Parse("{ '$schema': 'fake' }"),
                    Parameters = new Dictionary<string, object>
                    {
                        ["param1"] = "foo",
                        ["param2"] = new Dictionary<string, object>
                        {
                            ["param2_1"] = 123,
                            ["param2_2"] = "bar"
                        }
                    }
                })
            {
                Location = "westus"
            };

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAtSubscriptionScopeAsync("test-subscription-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var resultJson = JObject.Parse(resquestContent);
            Assert.AreEqual("westus", resultJson["location"].Value<string>());
            Assert.AreEqual("Complete", resultJson["properties"]["mode"].Value<string>());
            Assert.AreEqual("https://example.com", resultJson["properties"]["templateLink"]["uri"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["templateLink"]["contentVersion"].Value<string>());
            Assert.AreEqual("https://example.com/parameters", resultJson["properties"]["parametersLink"]["uri"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["parametersLink"]["contentVersion"].Value<string>());
            Assert.AreEqual(JObject.Parse("{ '$schema': 'fake' }"), resultJson["properties"]["template"]);
            Assert.AreEqual("foo", resultJson["properties"]["parameters"]["param1"].Value<string>());
            Assert.AreEqual(123, resultJson["properties"]["parameters"]["param2"]["param2_1"].Value<int>());
            Assert.AreEqual("bar", resultJson["properties"]["parameters"]["param2"]["param2_2"].Value<string>());
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_SendingRequestWithStrings_SerializesPayload()
        {
            // Arrange.
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            string templateContent = "{" +
                "\"$schema\": \"http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#\"," +
                "\"contentVersion\": \"1.0.0.0\"," +
                "\"parameters\": {" +
                    "\"roleDefName\": {" +
                        "\"type\": \"string\"" +
                    "}" +
                "}," +
                "\"resources\": [" +
                "]," +
                "\"outputs\": {}" +
            "}";
            string parameterContent = "{" +
                "\"roleDefName\": {" +
                    "\"value\": \"myCustomRole\"" +
                "}" +
            "}";
            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(templateContent);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(parameterContent);
            object template = jsonTemplate.GetObject();
            object parameter = jsonParameter.GetObject();

            var deploymentWhatIf = new DeploymentWhatIf(
                new DeploymentWhatIfProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = parameter
                })
            {
                Location = "westus"
            };

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            await client.GetDeploymentsClient().StartWhatIfAtSubscriptionScopeAsync("test-subscription-deploy", deploymentWhatIf);

            // Assert.
            var request = mockTransport.Requests[0];
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var resultJson = JObject.Parse(resquestContent);
            Assert.AreEqual("Incremental", resultJson["properties"]["mode"].Value<string>());
            Assert.AreEqual("1.0.0.0", resultJson["properties"]["template"]["contentVersion"].Value<string>());
            Assert.AreEqual("myCustomRole", resultJson["properties"]["parameters"]["roleDefName"]["value"].Value<string>());
        }

        [Test]
        public async Task WhatIfAtSubscriptionScope_ReceivingResponse_DeserializesResult()
        {
            // Arrange.
            var deploymentWhatIf = new DeploymentWhatIf(new DeploymentWhatIfProperties(DeploymentMode.Incremental));
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                    'status': 'Succeeded',
                    'properties': {
                        'changes': [
                            {
                                'resourceId': '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/myResourceGroup',
                                'changeType': 'Create',
                                'after': {
                                    'apiVersion': '2019-05-01',
                                    'id': '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/myResourceGroup',
                                    'type': 'Microsoft.Resources/resourceGroups',
                                    'name': 'myResourceGroup',
                                    'location': 'eastus',
                                }
                            }
                        ]
                    }
                }").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            // Act.
            var raw = await client.GetDeploymentsClient().StartWhatIfAtSubscriptionScopeAsync("test-subscription-deploy", deploymentWhatIf);
            var result = (await raw.WaitForCompletionAsync()).Value;


            // Assert.
            Assert.AreEqual("Succeeded", result.Status);
            Assert.NotNull(result.Changes);
            Assert.AreEqual(1, result.Changes.Count);

            WhatIfChange change = result.Changes[0];
            Assert.AreEqual(ChangeType.Create, change.ChangeType);

            Assert.Null(change.Before);
            Assert.Null(change.Delta);

            Assert.NotNull(change.After);
            Assert.AreEqual("myResourceGroup", JToken.FromObject(change.After)["name"].Value<string>());
        }
    }
}
