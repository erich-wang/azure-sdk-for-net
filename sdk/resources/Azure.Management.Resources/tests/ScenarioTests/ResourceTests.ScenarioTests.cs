// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Resources.Tests;
using NUnit.Framework;

namespace ResourceGroups.Tests
{
    public class LiveResourceTests : ResourceOperationsTestsBase
    {
        public const string WebResourceProviderVersion = "2018-02-01";
        public const string SendGridResourceProviderVersion = "2015-01-01";

        public LiveResourceTests(bool isAsync)
            : base(isAsync)
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
            string groupName = Recording.GenerateAssetName("csmrg");
            string resourceName = Recording.GenerateAssetName("csmr");
            string password = Recording.GenerateAssetName("p@ss");
            string mySqlLocation = "centralus";
            string resourceProviderNamespace = "Sendgrid.Email";
            string resourceType = "accounts";
            //SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup("centralus"));
            var rawCreateOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                resourceProviderNamespace,
                "",
                resourceType,
                resourceName,
                SendGridResourceProviderVersion,
                   new GenericResource
                   {
                       Location = mySqlLocation,
                       Plan = new Plan { Name = "free", Publisher = "Sendgrid", Product = "sendgrid_azure", PromotionCode = "" },
                       Tags = new Dictionary<string, string> { { "provision_source", "RMS" } },
                       Properties = new Dictionary<string, object>
                                {
                                    {
                                        "password", password
                                    },
                                    {
                                        "acceptMarketingEmails", false
                                    },
                                    {
                                         "email", "tiano@email.com"
                                    }
                                }
                   }
               );
            var createOrUpdateResult = (await WaitForCompletionAsync(rawCreateOrUpdateResult)).Value;

            Assert.True(Utilities.LocationsAreEqual(mySqlLocation, createOrUpdateResult.Location),
                string.Format("Resource location for resource '{0}' does not match expected location '{1}'", createOrUpdateResult.Location, mySqlLocation));
            Assert.NotNull(createOrUpdateResult.Plan);
            Assert.AreEqual("free", createOrUpdateResult.Plan.Name);

            var getResult = await ResourcesClient.GetAsync(groupName, resourceProviderNamespace,
                "", resourceType, resourceName, SendGridResourceProviderVersion);

            Assert.AreEqual(resourceName, getResult.Value.Name);
            Assert.True(Utilities.LocationsAreEqual(mySqlLocation, getResult.Value.Location),
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

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(this.ResourceGroupLocation));
            var rawCreateOrUpdateResult = await ResourcesClient.StartCreateOrUpdateAsync(groupName, "Microsoft.Web", "", "serverFarms", resourceName, WebResourceProviderVersion,
                new GenericResource()
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );

            var createOrUpdateResult = (await WaitForCompletionAsync(rawCreateOrUpdateResult)).Value;

            Assert.NotNull(createOrUpdateResult.Id);
            Assert.AreEqual(resourceName, createOrUpdateResult.Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Utilities.LocationsAreEqual(location, createOrUpdateResult.Location),
                string.Format("Resource location for website '{0}' does not match expected location '{1}'", createOrUpdateResult.Location, location));
            ;

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.IsTrue(listResult.Count() == 1);
            //Assert.Single(listResult);
            Assert.AreEqual(resourceName, listResult.First().Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Utilities.LocationsAreEqual(location, listResult.First().Location),
                string.Format("Resource list location for website '{0}' does not match expected location '{1}'", listResult.First().Location, location));

            var listResult2 = ResourcesClient.ListByResourceGroupAsync(groupName, null, null, 10).ToEnumerableAsync();

            Assert.IsTrue(listResult.Count() == 1);
            //Assert.Single(listResult);
            Assert.AreEqual(resourceName, listResult2.Result.First().Name);
            Assert.True(string.Equals("Microsoft.Web/serverFarms", createOrUpdateResult.Type, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Utilities.LocationsAreEqual(location, listResult.First().Location),
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
            string filter = "tagName eq '" + tagName + "'";

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(this.ResourceGroupLocation));
            await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                string.Empty,
                "serverFarms",
                resourceName,
                WebResourceProviderVersion,
                new GenericResource
                {
                    Tags = new Dictionary<string, string> { { tagName, "" } },
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                });
            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(15 * 1000);

            await ResourcesClient.StartCreateOrUpdateAsync(
                    groupName,
                    "Microsoft.Web",
                    string.Empty,
                    "serverFarms",
                    resourceNameNoTags,
                    WebResourceProviderVersion,
                    new GenericResource
                    {
                        Location = location,
                        Sku = new Sku
                        {
                            Name = "S1"
                        },
                        Properties = new Dictionary<string, object> { }
                    });
            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(15 * 1000);

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName, filter, null, null).ToEnumerableAsync();

            Assert.IsTrue(listResult.Count() == 1);
            Assert.AreEqual(resourceName, listResult.First().Name);

            var getResult = await ResourcesClient.GetAsync(
                groupName,
                "Microsoft.Web",
                string.Empty,
                "serverFarms",
                resourceName,
                WebResourceProviderVersion);

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
            string filter = "tagName eq '" + tagName + "' and tagValue eq '" + tagValue + "'";

            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(this.ResourceGroupLocation));
            await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceName,
                WebResourceProviderVersion,
                new GenericResource
                {
                    Tags = new Dictionary<string, string> { { tagName, tagValue } },
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );
            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(15 * 1000);

            await ResourcesClient.StartCreateOrUpdateAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceNameNoTags,
                WebResourceProviderVersion,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );
            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(15 * 1000);

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName, filter, null, null).ToEnumerableAsync();

            Assert.IsTrue(listResult.Count() == 1);
            Assert.AreEqual(resourceName, listResult.First().Name);

            var getResult = await ResourcesClient.GetAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverFarms",
                resourceName,
                WebResourceProviderVersion);

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
                WebResourceProviderVersion,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.AreEqual(resourceName, listResult.First().Name);

            await ResourcesClient.StartDeleteAsync(
                groupName,
                "Microsoft.Web",
                "",
                "serverfarms",
                resourceName,
                WebResourceProviderVersion);
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
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(location));
            var createOrUpdateResult = await ResourcesClient.StartCreateOrUpdateByIdAsync(
                resourceId,
                WebResourceProviderVersion,
                new GenericResource
                {
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );

            var listResult = await ResourcesClient.ListByResourceGroupAsync(groupName).ToEnumerableAsync();

            Assert.AreEqual(resourceName, listResult.First().Name);

            await ResourcesClient.StartDeleteByIdAsync(
                resourceId, WebResourceProviderVersion);
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
                WebResourceProviderVersion,
                new GenericResource
                {
                    Tags = new Dictionary<string, string>() { { "department", "finance" }, { "tagname", "tagvalue" } },
                    Location = location,
                    Sku = new Sku
                    {
                        Name = "S1"
                    },
                    Properties = new Dictionary<string, object> { }
                }
            );

            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(20 * 1000);

            var listResult = await ResourcesClient.ListAsync().ToEnumerableAsync();
            var filter = listResult.Find(item => item.Name == resourceName);
            Assert.AreEqual(2, filter.Tags.Count);
        }

        private string ResourceGroupLocation
        {
            get { return "South Central US"; }
        }

        public string GetWebsiteLocation()
        {
            return "West US";
        }
    }
}
