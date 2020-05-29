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
    public class InMemoryResourceTests : ResourceOperationsTestsBase
    {
        public InMemoryResourceTests(bool isAsync) : base(isAsync)
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
        public async Task ResourceGetValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                  'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                  'name': 'site1',
                  'location': 'South Central US',
                   'properties': {
                        'name':'site1',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated',
                        'provisioningState':'Succeeded'
                   },
                   'sku': {
                        'name': 'F1',
                        'tier': 'Free',
                        'size': 'F1',
                        'family': 'F',
                        'capacity': 0
                    }
                }").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = (await client.GetResourcesClient().GetAsync("foo", "Microsoft.Web", string.Empty, "Sites", "site1", "2014-01-04")).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
            Assert.AreEqual("site1", result.Name);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.Id);
            Assert.AreEqual("Dedicated", JObject.FromObject(result.Properties)["computeMode"].Value<string>());
            Assert.AreEqual("Succeeded", JObject.FromObject(result.Properties)["provisioningState"].Value<string>());
            Assert.AreEqual("F1", result.Sku.Name);
            Assert.AreEqual("Free", result.Sku.Tier);
            Assert.AreEqual("F1", result.Sku.Size);
            Assert.AreEqual("F", result.Sku.Family);
            Assert.AreEqual(0, result.Sku.Capacity);
        }

        [Test]
        public async Task ResourceGetByIdValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                  'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                  'name': 'site1',
                  'location': 'South Central US',
                   'properties': {
                        'name':'site1',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated',
                        'provisioningState':'Succeeded'
                   },
                   'sku': {
                        'name': 'F1',
                        'tier': 'Free',
                        'size': 'F1',
                        'family': 'F',
                        'capacity': 0
                    }
                }").ToString();

            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = (await client.GetResourcesClient().GetByIdAsync("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", "2014-01-04")).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
            Assert.AreEqual("site1", result.Name);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.Id);
            Assert.AreEqual("Dedicated", JObject.FromObject(result.Properties)["computeMode"].Value<string>());
            Assert.AreEqual("Succeeded", JObject.FromObject(result.Properties)["provisioningState"].Value<string>());
            Assert.AreEqual("F1", result.Sku.Name);
            Assert.AreEqual("Free", result.Sku.Tier);
            Assert.AreEqual("F1", result.Sku.Size);
            Assert.AreEqual("F", result.Sku.Family);
            Assert.AreEqual(0, result.Sku.Capacity);
        }

        [Test]
        public async Task ResourceGetWorksWithoutProvisioningState()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                  'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                  'name': 'site1',
                  'location': 'South Central US',
                   'properties': {
                        'name':'site1',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated'
                    }
                }").ToString();

            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = "site1";
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "Sites";
            var result = (await client.GetResourcesClient().GetAsync(
                "foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion)).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
            Assert.AreEqual("site1", result.Name);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.Id);
            Assert.AreEqual("Dedicated", JObject.FromObject(result.Properties)["computeMode"].Value<string>());
            Assert.Null(JObject.FromObject(result.Properties)["provisionState"]);
        }

        [Test]
        public async Task ResourceListValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{ 'value' : [
                    {
                      'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                      'name': 'site1',
                      'resourceGroup': 'foo',
                      'location': 'South Central US',
                      'properties': 
                       { 
                          'name':'site1',
	                      'siteMode': 'Standard',
                          'computeMode':'Dedicated',
                          'provisioningState':'Succeeded'
                       }
                    },
                    {
                      'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                      'name': 'site1',
                      'resourceGroup': 'foo',
                      'location': 'South Central US',
                      'properties': 
                       { 
                          'name':'site1',
	                      'siteMode': 'Standard',
                          'computeMode':'Dedicated'
                       }
                    }]}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = await client.GetResourcesClient().ListByResourceGroupAsync("foo", "$filter=resourceType eq 'Sites'").ToEnumerableAsync();

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("South Central US", result.First().Location);
            Assert.AreEqual("site1", result.First().Name);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.First().Id);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.First().Id);
            Assert.AreEqual("Dedicated", JObject.FromObject(result.First().Properties)["computeMode"].Value<string>());
            Assert.AreEqual("Succeeded", JObject.FromObject(result.First().Properties)["provisioningState"].Value<string>());
        }

        [Test]
        public async Task ResourceListForResourceGroupValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{ 'value' : [
                    {
                      'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                      'name': 'site1',
                      'resourceGroup': 'foo',
                      'location': 'South Central US'
                    },
                    {
                      'id': '/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1',
                      'name': 'site1',
                      'resourceGroup': 'foo',
                      'location': 'South Central US'
                    }]}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = await client.GetResourcesClient().ListByResourceGroupAsync("foo", "$filter=resourceType eq 'Sites'").ToEnumerableAsync();


            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Get.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("South Central US", result.First().Location);
            Assert.AreEqual("site1", result.First().Name);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.First().Id);
            Assert.AreEqual("/subscriptions/12345/resourceGroups/foo/providers/Microsoft.Web/Sites/site1", result.First().Id);
            Assert.Null(result.First().Properties);
        }

        [Test]
        public async Task ResourceGetThrowsExceptions()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NoContent);
            var content = JObject.Parse("{}").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            try
            {
                await client.GetResourcesClient().GetAsync(null, null, null, null, null, null);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }

        [Test]
        public async Task ResourceCreateOrUpdateValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                    'location': 'South Central US',
                    'tags' : {
                        'department':'finance',
                        'tagname':'tagvalue'
                    },
                    'properties': {
                        'name':'site3',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated',
                        'provisioningState':'Succeeded'
                    }
                }").ToString();

            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var raw = await client.GetResourcesClient().StartCreateOrUpdateAsync(
                "foo",
                "Microsoft.Web",
                string.Empty,
                "sites",
                "site3",
                "2014-01-04",
                new GenericResource
                {
                    Location = "South Central US",
                    Tags = new Dictionary<string, string>() { { "department", "finance" }, { "tagname", "tagvalue" } },
                    Properties = @"{
                        'name':'site3',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated'
                    }"
                }
            );
            var result = (await raw.WaitForCompletionAsync()).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Put.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate payload
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var json = JObject.Parse(resquestContent);
            Assert.AreEqual("South Central US", json["location"].Value<string>());
            Assert.AreEqual("finance", json["tags"]["department"].Value<string>());
            Assert.AreEqual("tagvalue", json["tags"]["tagname"].Value<string>());

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
            Assert.AreEqual("Succeeded", JObject.FromObject(result.Properties)["provisioningState"].Value<string>());
            Assert.AreEqual("Dedicated", JObject.FromObject(result.Properties)["computeMode"].Value<string>());
            Assert.AreEqual("finance", result.Tags["department"]);
            Assert.AreEqual("tagvalue", result.Tags["tagname"]);
        }

        [Test]
        public async Task ResourceCreateOrUpdateWithIdentityValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse(@"{
                    'location': 'South Central US',
                    'tags' : {
                        'department':'finance',
                        'tagname':'tagvalue'
                    },
                    'properties': {
                        'name':'site3',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated',
                        'provisioningState':'Succeeded'
                    },
                    'identity': {
                        'type': 'SystemAssigned',
                        'principalId': 'foo'
                    }
                }").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var raw = await client.GetResourcesClient().StartCreateOrUpdateAsync(
                "foo",
                "Microsoft.Web",
                string.Empty,
                "sites",
                "site3",
                "2014-01-04",
                new GenericResource
                {
                    Location = "South Central US",
                    Tags = new Dictionary<string, string>() { { "department", "finance" }, { "tagname", "tagvalue" } },
                    Properties = @"{
                        'name':'site3',
	                    'siteMode': 'Standard',
                        'computeMode':'Dedicated'
                    }",
                    Identity = new Identity { Type = ResourceIdentityType.SystemAssigned }
                }
            );
            var result = (await raw.WaitForCompletionAsync()).Value;

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Put.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate payload
            Stream stream = new MemoryStream();
            await request.Content.WriteToAsync(stream, default);
            stream.Position = 0;
            var resquestContent = new StreamReader(stream).ReadToEnd();
            var json = JObject.Parse(resquestContent);
            Assert.AreEqual("South Central US", json["location"].Value<string>());
            Assert.AreEqual("finance", json["tags"]["department"].Value<string>());
            Assert.AreEqual("tagvalue", json["tags"]["tagname"].Value<string>());

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
            Assert.AreEqual("Succeeded", JObject.FromObject(result.Properties)["provisioningState"].Value<string>());
            Assert.AreEqual("finance", result.Tags["department"]);
            Assert.AreEqual("tagvalue", result.Tags["tagname"]);
            Assert.AreEqual("Dedicated", JObject.FromObject(result.Properties)["computeMode"].Value<string>());
            Assert.AreEqual("SystemAssigned", result.Identity.Type.ToString());
            Assert.AreEqual("foo", result.Identity.PrincipalId);
        }

        [Test]
        public async Task ResourceCreateForWebsiteValidatePayload()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse("{'location':'South Central US','tags':null,'properties':{'name':'csmr14v5efk0','state':'Running','hostNames':['csmr14v5efk0.antares-int.windows-int.net'],'webSpace':'csmrgqinwpwky-SouthCentralUSwebspace','selfLink':'https://antpreview1.api.admin-antares-int.windows-int.net:454/20130801/websystems/websites/web/subscriptions/abc123/webspaces/csmrgqinwpwky-SouthCentralUSwebspace/sites/csmr14v5efk0','repositorySiteName':'csmr14v5efk0','owner':null,'usageState':0,'enabled':true,'adminEnabled':true,'enabledHostNames':['csmr14v5efk0.antares-int.windows-int.net','csmr14v5efk0.scm.antares-int.windows-int.net'],'siteProperties':{'metadata':null,'properties':[],'appSettings':null},'availabilityState':0,'sslCertificates':[],'csrs':[],'cers':null,'siteMode':'Standard','hostNameSslStates':[{'name':'csmr14v5efk0.antares-int.windows-int.net','sslState':0,'ipBasedSslResult':null,'virtualIP':null,'thumbprint':null,'toUpdate':null,'toUpdateIpBasedSsl':null,'ipBasedSslState':0},{'name':'csmr14v5efk0.scm.antares-int.windows-int.net','sslState':0,'ipBasedSslResult':null,'virtualIP':null,'thumbprint':null,'toUpdate':null,'toUpdateIpBasedSsl':null,'ipBasedSslState':0}],'computeMode':1,'serverFarm':'DefaultServerFarm1','lastModifiedTimeUtc':'2014-02-21T00:49:30.477','storageRecoveryDefaultState':'Running','contentAvailabilityState':0,'runtimeAvailabilityState':0,'siteConfig':null,'deploymentId':'csmr14v5efk0','trafficManagerHostNames':[]}}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var raw = await client.GetResourcesClient().StartCreateOrUpdateAsync(
                "foo",
                "Microsoft.Web",
                string.Empty,
                "sites",
                "csmr14v5efk0",
                "2014-01-04",
                new GenericResource
                {
                    Location = "South Central US",
                    Properties = @"{
                            'name':'csmr14v5efk0',
	                        'siteMode': 'Standard',
                            'computeMode':'Dedicated'
                        }"
                }
                );
            var result = (await raw.WaitForCompletionAsync()).Value;

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
        }

        [Test]
        public async Task ResourceCreateByIdForWebsiteValidatePayload()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse("{'location':'South Central US','tags':null,'properties':{'name':'mySite','state':'Running','hostNames':['csmr14v5efk0.antares-int.windows-int.net'],'webSpace':'csmrgqinwpwky-SouthCentralUSwebspace','selfLink':'https://antpreview1.api.admin-antares-int.windows-int.net:454/20130801/websystems/websites/web/subscriptions/abc123/webspaces/csmrgqinwpwky-SouthCentralUSwebspace/sites/csmr14v5efk0','repositorySiteName':'csmr14v5efk0','owner':null,'usageState':0,'enabled':true,'adminEnabled':true,'enabledHostNames':['csmr14v5efk0.antares-int.windows-int.net','csmr14v5efk0.scm.antares-int.windows-int.net'],'siteProperties':{'metadata':null,'properties':[],'appSettings':null},'availabilityState':0,'sslCertificates':[],'csrs':[],'cers':null,'siteMode':'Standard','hostNameSslStates':[{'name':'csmr14v5efk0.antares-int.windows-int.net','sslState':0,'ipBasedSslResult':null,'virtualIP':null,'thumbprint':null,'toUpdate':null,'toUpdateIpBasedSsl':null,'ipBasedSslState':0},{'name':'csmr14v5efk0.scm.antares-int.windows-int.net','sslState':0,'ipBasedSslResult':null,'virtualIP':null,'thumbprint':null,'toUpdate':null,'toUpdateIpBasedSsl':null,'ipBasedSslState':0}],'computeMode':1,'serverFarm':'DefaultServerFarm1','lastModifiedTimeUtc':'2014-02-21T00:49:30.477','storageRecoveryDefaultState':'Running','contentAvailabilityState':0,'runtimeAvailabilityState':0,'siteConfig':null,'deploymentId':'csmr14v5efk0','trafficManagerHostNames':[]}}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var raw = await client.GetResourcesClient().StartCreateOrUpdateByIdAsync(
                "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myGroup/Microsoft.Web/sites/mySite",
                "2014-01-04",
                new GenericResource
                {
                    Location = "South Central US",
                    Properties = @"{
                            'name':'mySite',
	                        'siteMode': 'Standard',
                            'computeMode':'Dedicated'
                        }"
                }
                );
            var result = (await raw.WaitForCompletionAsync()).Value;

            // Validate result
            Assert.AreEqual("South Central US", result.Location);
        }

        [Test]
        public async Task ResourceGetCreateOrUpdateDeleteAndExistsThrowExceptionWithoutApiVersion()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse("{}").ToString();
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var resourceName = "site3";
            var resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = null;
            var resourceType = "sites";
            var parentResourse = string.Empty;

            var resource = new GenericResource
            {
                Location = "South Central US",
                Properties = @"{
                                'name':'site3',
	                            'siteMode': 'Standard',
                                'computeMode':'Dedicated'
                            }"
            };

            try
            {
                await client.GetResourcesClient().GetAsync(
                "foo",
                resourceProviderNamespace,
                parentResourse,
                resourceType,
                resourceName,
                resourceProviderApiVersion);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }

            try
            {
                await client.GetResourcesClient().CheckExistenceAsync(
                "foo",
                resourceProviderNamespace,
                parentResourse,
                resourceType,
                resourceName,
                resourceProviderApiVersion);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
            try
            {
                await client.GetResourcesClient().StartCreateOrUpdateAsync(
                "foo",
                resourceProviderNamespace,
                parentResourse,
                resourceType,
                resourceName,
                resourceProviderApiVersion,
                resource);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
            try
            {
                await client.GetResourcesClient().StartDeleteAsync(
                "foo",
                resourceProviderNamespace,
                parentResourse,
                resourceType,
                resourceName,
                resourceProviderApiVersion);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }


        [Test]
        public async Task ResourceExistsValidateNoContentMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NoContent);
            var content = JObject.Parse("{}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = "site3";
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "sites";
            var result = await client.GetResourcesClient().CheckExistenceAsync(
                "foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion);

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Head.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.NotNull(result);
        }

        [Test]
        public async Task ResourceExistsValidateMissingMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NotFound);
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = Guid.NewGuid().ToString();
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "sites";

            var result = await client.GetResourcesClient().CheckExistenceAsync(
                "foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion);

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Head.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));

            // Validate result
            Assert.NotNull(result);
        }

        [Test]
        public async Task UriSupportsBaseUriWithPathTest()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NotFound);
            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = Guid.NewGuid().ToString();
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "sites";
            await client.GetResourcesClient().CheckExistenceAsync("foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion);

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual("https://management.azure.com/subscriptions/" + TestEnvironment.SubscriptionId + "/resourcegroups/foo/providers/Microsoft.Web//sites/" + resourceName + "?api-version=2014-01-04", request.Uri.ToString());
        }

        [Test]
        public async Task ResourceDeleteValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse("{}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = "site3";
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "sites";
            await client.GetResourcesClient().StartDeleteAsync("foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion);

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Delete.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task ResourceDeleteByIdValidateMessage()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.OK);
            var content = JObject.Parse("{}").ToString();

            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            await client.GetResourcesClient().StartDeleteByIdAsync("/subscriptions/12345/resourceGroups/myGroup/Microsoft.Web/sites/mySite", "2014-01-04");

            // Validate headers
            var request = mockTransport.Requests[0];
            Assert.AreEqual(HttpMethod.Delete.Method, request.Method.Method);
            Assert.IsTrue(request.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task ResourceDeleteSupportNoContentReturnCode()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NoContent);
            var content = JObject.Parse("{}").ToString();
            var header = new HttpHeader("x-ms-request-id", "1");
            mockResponse.AddHeader(header);
            mockResponse.SetContent(content);

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            string resourceName = "site3";
            string resourceProviderNamespace = "Microsoft.Web";
            string resourceProviderApiVersion = "2014-01-04";
            string resourceType = "sites";

            try
            {
                await client.GetResourcesClient().StartDeleteAsync("foo",
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                resourceProviderApiVersion);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }
    }
}
