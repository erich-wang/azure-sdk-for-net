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
using System.Threading;
using Microsoft.Azure.Test;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using Microsoft.Rest.TransientFaultHandling;
using Azure.Management.Resource.Tests.Helper;

namespace ResourceGroups.Tests
{
    public class LiveResourceTests : ResourceOperationsTestsBase
    {
        public LiveResourceTests(bool isAsync, ResourceManagementClientOptions.ServiceVersion serviceVersion)
            : base(isAsync, serviceVersion)
        {
        }
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
        public async Task CreateResourceWithPlan()
        {
            //var handler = new RecordedDelegatingHandler() { StatusCodeToReturn = HttpStatusCode.OK };

            //using (MockContext context = MockContext.Start(this.GetType()))
            //{
                string groupName = Recording.GenerateAssetName("csmrg");
                string resourceName = Recording.GenerateAssetName("csmr");
                string password = Recording.GenerateAssetName("p@ss");
                //var client = GetResourceManagementClient(context, handler);
                string mySqlLocation = "centralus";
            var groupIdentity = new ResourceIdentity
            {
                ResourceName = resourceName,
                ResourceProviderNamespace = "Sendgrid.Email",
                ResourceType = "accounts",
                ResourceProviderApiVersion = SendGridResourceProviderVersion
            };
            //SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup("centralus"));
            var rawCreateOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(groupName, groupIdentity.ResourceProviderNamespace, "", groupIdentity.ResourceType,
                   groupIdentity.ResourceName,
                   new GenericResource
                   {
                       Location = mySqlLocation,
                       Plan = new Plan { Name = "free", Publisher = "Sendgrid", Product = "sendgrid_azure", PromotionCode = "" },
                       Tags = new Dictionary<string, string> { { "provision_source", "RMS" } },
                       Properties = JObject.Parse("{'password':'" + password + "','acceptMarketingEmails':false,'email':'tiano@email.com'}"),
                   }
               );
            var createOrUpdateResult = (await rawCreateOrUpdateResult.WaitForCompletionAsync()).Value;

                Assert.True(Helper.LocationsAreEqual(mySqlLocation, createOrUpdateResult.Location),
                    string.Format("Resource location for resource '{0}' does not match expected location '{1}'", createOrUpdateResult.Location, mySqlLocation));
                Assert.NotNull(createOrUpdateResult.Plan);
                Assert.AreEqual("free", createOrUpdateResult.Plan.Name);

                var getResult =await ResourcesClient.GetAsync(groupName, groupIdentity.ResourceProviderNamespace,
                    "", groupIdentity.ResourceType, groupIdentity.ResourceName);

                Assert.AreEqual(resourceName, getResult.Value.Name);
                Assert.True(Helper.LocationsAreEqual(mySqlLocation, getResult.Value.Location),
                    string.Format("Resource location for resource '{0}' does not match expected location '{1}'", getResult.Value.Location, mySqlLocation));
                Assert.NotNull(getResult.Value.Plan);
                Assert.AreEqual("free", getResult.Value.Plan.Name);
        }

        [Test]
        public async Task CreatedResourceIsAvailableInList()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");
            string location = GetWebsiteLocation();

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup (this.ResourceGroupLocation));
            var createOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(groupName, "Microsoft.Web", "", "serverFarms", resourceName,
                new GenericResource()
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );

            Assert.NotNull(createOrUpdateResult.Id);
            Assert.AreEqual(resourceName, createOrUpdateResult.Value.Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Value.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Helper.LocationsAreEqual(location, createOrUpdateResult.Value.Location),
                string.Format("Resource location for website '{0}' does not match expected location '{1}'", createOrUpdateResult.Value.Location, location));
            ;

            var listResult =await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.IsTrue(listResult.Count()==1);
            //Assert.Single(listResult);
            Assert.AreEqual(resourceName, listResult.First().Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Value.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Helper.LocationsAreEqual(location, listResult.First().Location),
                string.Format("Resource list location for website '{0}' does not match expected location '{1}'", listResult.First().Location, location));
            var listResult2 =ResourcesClient.ListByResourceGroupAsync(groupName, null, null, 10).ToEnumerableAsync();


            Assert.IsTrue(listResult.Count() == 1);
            //Assert.Single(listResult);
            Assert.AreEqual(resourceName, listResult2.Result.First().Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Value.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Helper.LocationsAreEqual(location, listResult.First().Location),
                string.Format("Resource list location for website '{0}' does not match expected location '{1}'", listResult.First().Location, location));
        }

        [Test]
        public async Task CreatedResourceIsAvailableInListFilteredByTagName()
        {
                string groupName = Recording.GenerateAssetName("csmrg");
                string resourceName = Recording.GenerateAssetName("csmr");
                string resourceNameNoTags = Recording.GenerateAssetName("csmr");
                string tagName = Recording.GenerateAssetName("csmtn");
                string location = GetWebsiteLocation();

                //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

                await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(this.ResourceGroupLocation));
                await ResourcesClient.StartCreateOrUpdateAsync(
                    groupName,
                    "Microsoft.Web",
                    string.Empty,
                    "serverFarms",
                    resourceName,
                    new GenericResource
                    {
                        Tags = new Dictionary<string, string> { { tagName, "" } },
                        Location = location,
                        Sku = new Sku
                        {
                            Name = "S1"
                        },
                        Properties = JObject.Parse("{}")
                    });
                await ResourcesClient.StartCreateOrUpdateAsync(
                    groupName,
                    "Microsoft.Web",
                    string.Empty,
                    "serverFarms",
                    resourceNameNoTags,
                    new GenericResource
                    {
                        Location = location,
                        Sku = new Sku
                        {
                            Name = "S1"
                        },
                        Properties = JObject.Parse("{}")
                    });

                var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName, tagName ,null,null).ToEnumerableAsync();

                Assert.IsTrue(listResult.Count()==1);
                Assert.AreEqual(resourceName, listResult.First().Name);

                var getResult =await ResourcesClient.GetAsync(
                    groupName,
                    "Microsoft.Web",
                    string.Empty,
                    "serverFarms",
                    resourceName);

                Assert.AreEqual(resourceName, getResult.Value.Name);
                Assert.True(getResult.Value.Tags.Keys.Contains(tagName));
        }

        [Test]
        public async Task CreatedResourceIsAvailableInListFilteredByTagNameAndValue()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");
            string resourceNameNoTags = Recording.GenerateAssetName("csmr");
            string tagName = Recording.GenerateAssetName("csmtn");
            string tagValue = Recording.GenerateAssetName("csmtv");
            string location = GetWebsiteLocation();

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(this.ResourceGroupLocation));
            await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceName,
                new GenericResource
                {
                    Tags = new Dictionary<string, string> { { tagName, tagValue } },
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );
            await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceNameNoTags,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );
            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName,null,null,null).ToEnumerableAsync();
            //var listResult = ResourcesClient.ListByResourceGroup(groupName,
            //    new ODataQuery<GenericResourceFilter>(r => r.Tagname == tagName && r.Tagvalue == tagValue));

            Assert.IsTrue(listResult.Count()==1);
            Assert.AreEqual(resourceName, listResult.First().Name);

            var getResult = await ResourcesClient.GetAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceName);

            Assert.AreEqual(resourceName, getResult.Value.Name);
            Assert.True(getResult.Value.Tags.Keys.Contains(tagName));
        }

        [Test]
        public async Task CreatedAndDeleteResource()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));
            string location = this.GetWebsiteLocation();
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(location));
            var createOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverfarms",
                resourceName,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.AreEqual(resourceName, listResult.First().Name);

            await ResourcesClient.StartDeleteAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverfarms",
                resourceName);
        }

        [Test]
        public async Task CreatedAndDeleteResourceById()
        {
            string subscriptionId = "b9f138a1-1d64-4108-8413-9ea3be1c1b2d";
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));
            string location = this.GetWebsiteLocation();

            string resourceId = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Web/serverFarms/{2}", subscriptionId, groupName, resourceName);
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup (location));
            var createOrUpdateResult = await ResourcesClient.StartCreateOrUpdateByIdAsync(
                resourceId,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.AreEqual(resourceName, listResult.First().Name);

            await ResourcesClient.StartDeleteByIdAsync(
                resourceId);
        }

        [Test]
        public async Task CreatedAndListResource()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");
            string location = this.GetWebsiteLocation();
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(location));
            var createOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceName,
                new GenericResource
                {
                    Tags = new Dictionary<string, string>() { { "department", "finance" }, { "tagname", "tagvalue" } },
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = JObject.Parse("{}")
                }
            );

            //var listResult = ResourcesClient.ListAsync(new ODataQuery<GenericResourceFilter>(r => r.ResourceType == "Microsoft.Web/serverFarms"));
            var listResult =await ResourcesClient.ListAsync().ToEnumerableAsync();
            //Assert.NotEmpty(listResult);
            Assert.AreEqual(2, listResult.First().Tags.Count);
        }

        private const string WebResourceProviderVersion = "2018-02-01";
        private const string SendGridResourceProviderVersion = "2015-01-01";

        private string ResourceGroupLocation
        {
            get { return "South Central US"; }
        }

        public static ResourceIdentity CreateResourceIdentity(GenericResource resource)
        {
            string[] parts = resource.Type.Split('/');
            return new ResourceIdentity { ResourceType = parts[1], ResourceProviderNamespace = parts[0], ResourceName = resource.Name, ResourceProviderApiVersion = WebResourceProviderVersion };
        }

        public string GetWebsiteLocation()
        {
            return "West US";
        }

        //public string GetMySqlLocation(ResourceManagementClient client)
        //{
        //    return Helper.GetResourceLocation(client, "SuccessBricks.ClearDB/databases");
        //}
    }
}
