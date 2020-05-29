using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
    public class InMemoryResourceLinkTests : ResourceOperationsTestsBase
    {
        public InMemoryResourceLinkTests(bool isAsync) : base(isAsync)
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
        public async Task ResourceLinkCRUD()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.Created);
            var content = JObject.Parse(@"{
                    'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink',
                    'name': 'myLink',
                    'properties': {
                        'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm',
                        'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2',
                        'notes': 'myLinkNotes'
                      }
                }").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = (await client.GetResourceLinksClient().CreateOrUpdateAsync(
                linkId: "/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink",
                parameters: new ResourceLink
                {
                    Properties = new ResourceLinkProperties("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2")
                    {
                        Notes = "myLinkNotes"
                    }
                })).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.IsTrue(request.Headers.Contains(new HttpHeader("Content-Type", "application/json")));
            Assert.AreEqual(HttpMethod.Put.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate payload
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var json = JObject.Parse(resquestContent);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", json["properties"]["targetId"].Value<string>());
            Assert.AreEqual("myLinkNotes", json["properties"]["notes"].Value<string>());

            // Validate response
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink", result.Id);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm", result.Properties.SourceId);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", result.Properties.TargetId);
            Assert.AreEqual("myLinkNotes", result.Properties.Notes);
            Assert.AreEqual("myLink", result.Name);

            //Get resource link and validate properties

            mockResponse = new MockResponse((int)HttpStatusCode.OK);
            content = JObject.Parse(@"{
                    'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink',
                    'name': 'myLink',
                    'properties': {
                        'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm',
                        'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2',
                        'notes': 'myLinkNotes'
                      }
                }").ToString();
            mockResponse.SetContent(content);

            mockTransport = new MockTransport(mockResponse);
            client = GetResourceManagementClient(mockTransport);

            var getResult = (await client.GetResourceLinksClient().GetAsync("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink")).Value;

            // Validate response
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink", getResult.Id);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm", getResult.Properties.SourceId);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", getResult.Properties.TargetId);
            Assert.AreEqual("myLinkNotes", getResult.Properties.Notes);
            Assert.AreEqual("myLink", getResult.Name);

            //Delete resource link
            mockResponse = new MockResponse((int)HttpStatusCode.OK);
            content = JObject.Parse("{}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            mockTransport = new MockTransport(mockResponse);
            client = GetResourceManagementClient(mockTransport);

            await client.GetResourceLinksClient().DeleteAsync("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink");

            // Validate headers
            request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Delete.Method, request.Method.Method);

            try
            {
                await client.GetResourceLinksClient().GetAsync("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink");
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }

        [Test]
        public async Task ResourceLinkList()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                    'value': [
                        {
                            'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink',
                            'name': 'myLink',
                            'properties': {
                                'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm',
                                'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2',
                                'notes': 'myLinkNotes'
                              }
                         },
                         {
                            'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Storage/storageAccounts/myAccount/providers/Microsoft.Resources/links/myLink2',
                            'name': 'myLink2',
                            'properties': {
                                'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Storage/storageAccounts/myAccount',
                                'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Storage/storageAccounts/myAccount2',
                                'notes': 'myLinkNotes2'
                              }
                         }]}").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = await client.GetResourceLinksClient().ListAtSubscriptionAsync().ToEnumerableAsync();

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);

            // Validate result
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("myLink", result.First().Name);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink", result.First().Id);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm", result.First().Properties.SourceId);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", result.First().Properties.TargetId);
            Assert.AreEqual("myLinkNotes", result.First().Properties.Notes);

            // Filter by targetId

            mockResponse = new MockResponse((int)HttpStatusCode.OK);
            content = JObject.Parse(@"{
                    'value': [
                        {
                            'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink',
                            'name': 'myLink',
                            'properties': {
                                'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm',
                                'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2',
                                'notes': 'myLinkNotes'
                              }
                         }]}").ToString();
            mockResponse.SetContent(content);

            mockTransport = new MockTransport(mockResponse);
            client = GetResourceManagementClient(mockTransport);
            var filteredResult = await client.GetResourceLinksClient().ListAtSubscriptionAsync("$filter=targetId eq '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2'").ToEnumerableAsync();

            // Validate result
            Has.One.EqualTo(filteredResult);
            Assert.AreEqual("myLink", filteredResult.First().Name);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink", filteredResult.First().Id);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm", filteredResult.First().Properties.SourceId);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", filteredResult.First().Properties.TargetId);
            Assert.AreEqual("myLinkNotes", filteredResult.First().Properties.Notes);
        }

        [Test]
        public async Task ResourceLinkListAtSourceScope()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                    'value': [
                        {
                            'id': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink',
                            'name': 'myLink',
                            'properties': {
                                'sourceId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm',
                                'targetId': '/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2',
                                'notes': 'myLinkNotes'
                              }
                         }]}").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = await client.GetResourceLinksClient().ListAtSourceScopeAsync("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm").ToEnumerableAsync();

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);

            // Validate result
            Has.One.EqualTo(result);
            Assert.AreEqual("myLink", result.First().Name);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm/providers/Microsoft.Resources/links/myLink", result.First().Id);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm", result.First().Properties.SourceId);
            Assert.AreEqual("/subscriptions/abc123/resourcegroups/myGroup/providers/Microsoft.Web/serverFarms/myFarm2", result.First().Properties.TargetId);
            Assert.AreEqual("myLinkNotes", result.First().Properties.Notes);
        }
    }
}
