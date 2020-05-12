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

namespace ResourceGroups.Tests
{
    public class LiveTagsTests : ResourceOperationsTestsBase
    {
        public LiveTagsTests(bool isAsync, ResourceManagementClientOptions.ServiceVersion serviceVersion)
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
        public async Task CreateListAndDeleteSubscriptionTag()
        {
            string tagName = Recording.GenerateAssetName("csmtg");
            var createResult = (await TagsClient.CreateOrUpdateAsync(tagName)).Value;
            Assert.AreEqual(tagName, createResult.TagName);

            var listResult =await TagsClient.ListAsync().ToEnumerableAsync();
            Assert.True(listResult.Count() > 0);

            await TagsClient.DeleteAsync(tagName);
        }

        [Test]
        public async Task CreateListAndDeleteSubscriptionTagValue()
        {
            string tagName = Recording.GenerateAssetName("csmtg");
            string tagValue = Recording.GenerateAssetName("csmtgv");

            var createNameResult = await TagsClient.CreateOrUpdateAsync(tagName);
            var createValueResult = await TagsClient.CreateOrUpdateValueAsync(tagName, tagValue);
            Assert.AreEqual(tagName, createNameResult.Value.TagName);
            Assert.AreEqual(tagValue, createValueResult.Value.TagValueValue);
            //Assert.AreEqual(tagValue, createValueResult.TagValueProperty);

            var listResult =await TagsClient.ListAsync().ToEnumerableAsync();
            Assert.True(listResult.Count() > 0);

            await TagsClient.DeleteValueAsync(tagName, tagValue);
            await TagsClient.DeleteAsync(tagName);
        }

        //[Test]
        //public async Task CreateTagValueWithoutCreatingTag()
        //{
        //    string tagName = Recording.GenerateAssetName("csmtg");
        //    string tagValue = Recording.GenerateAssetName("csmtgv");
        //    Assert.Throws<CloudException>(() => await TagsClient.CreateOrUpdateValueAsync(tagName, tagValue));
        //    //Assert.Throws<CloudException>(() => client.Tags.CreateOrUpdateValue(tagName, tagValue));
        //}


        /// <summary>
        /// Utility method to test Put request for Tags Operation within tracked resources and proxy resources
        /// </summary>
        private async void CreateOrUpdateTagsTest(string resourceScope)
        {
            var tagsResource = new TagsResource(new Tags()
            {
                TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue1" },
                    { "tagKey2", "tagValue2" }
                }
            }
            );
            // test creating tags for resources
            var putResponse =(await TagsClient.CreateOrUpdateAtScopeAsync(resourceScope, tagsResource.Properties)).Value;
            //putResponse.Properties.TagsValue.Count(tagsResource.Properties.TagsValue.Count());
            Assert.AreEqual(putResponse.Properties.TagsValue.Count(), tagsResource.Properties.TagsValue.Count());
            //putResponse.Properties.TagsProperty.Should().HaveCount(tagsResource.Properties.TagsProperty.Count);
            //this.CompareTagsResource(tagsResource, putResponse).Should().BeTrue();
            Assert.IsTrue(CompareTagsResource(tagsResource, putResponse));
        }

        /// <summary>
        /// Put request for Tags Operation within tracked resources
        /// </summary>
        [Test]
        public void  CreateTagsWithTrackedResourcesTest()
        {
            // test tags for tracked resources
            string resourceScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a/resourcegroups/TagsApiSDK/providers/Microsoft.Compute/virtualMachines/TagTestVM";
            CreateOrUpdateTagsTest(resourceScope);
        }

        /// <summary>
        /// Put request for Tags Operation within subscription test
        /// </summary>
        [Test]
        public void CreateTagsWithSubscriptionTest()
        {
            // test tags for subscription
            string subscriptionScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a";
            CreateOrUpdateTagsTest(subscriptionScope);
        }

        /// <summary>
        /// Utility method to test Patch request for Tags Operation within tracked resources and proxy resources, including Replace|Merge|Delete operations
        /// </summary>
        private async void UpdateTagsTest(string resourceScope)
        {
            // using Tags.CreateOrUpdateAtScope to create two tags initially
            var tagsResource = new TagsResource(new Tags()
            {
                TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue1" },
                    { "tagKey2", "tagValue2" }
                }
            }
            );
            await TagsClient.CreateOrUpdateAtScopeAsync(resourceScope, tagsResource.Properties);
            //var createorupdateAtSopeResponse= await TagsClient.CreateOrUpdateAtScopeAsync(resourceScope, tagsResource.Properties)).Value;
            Thread.Sleep(3000);

            var putTags = new Tags()
            {
                TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue3" },
                    { "tagKey3", "tagValue3" }
                }
            };

            { // test for Merge operation
                var tagPatchRequest = TagsPatchResourceOperation.Merge;
                //{
                //    Operation = "Merge",
                //    Properties = putTags
                //};
                var patchResponse =(await TagsClient.UpdateAtScopeAsync(resourceScope, tagPatchRequest, putTags)).Value;

                var expectedResponse = new TagsResource(new Tags()
                {
                    TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue3" },
                    { "tagKey2", "tagValue2" },
                    { "tagKey3", "tagValue3" }
                    }
                }
                );
                //patchResponse.Properties.Should().HaveCount(expectedResponse.Properties.TagsProperty.Count);
                Assert.AreEqual(patchResponse.Properties.TagsValue.Count(), expectedResponse.Properties.TagsValue.Count());
                //this.CompareTagsResource(expectedResponse, patchResponse).Should().BeTrue();
                Assert.IsTrue(this.CompareTagsResource(expectedResponse, patchResponse));
            }

            { // test for Replace operation
                var tagPatchRequest = TagsPatchResourceOperation.Replace;
                var patchResponse = await TagsClient.UpdateAtScopeAsync(resourceScope, tagPatchRequest);

                var expectedResponse = new TagsResource(putTags);
                //patchResponse.Value.Properties.TagsValue.Should().HaveCount(expectedResponse.Properties.TagsProperty.Count);
                Assert.AreEqual(patchResponse.Value.Properties.TagsValue.Count(), expectedResponse.Properties.TagsValue.Count());
                Assert.IsTrue(this.CompareTagsResource(expectedResponse, patchResponse));
                //this.CompareTagsResource(expectedResponse, patchResponse).Should().BeTrue();
            }

            { // test for Delete operation

                //var tagPatchRequest = new TagsPatchResource("Delete", putTags);
                //var patchResponse = client.Tags.UpdateAtScope(resourceScope, tagPatchRequest);
                var tagPatchRequest = TagsPatchResourceOperation.Delete;
                var patchResponse = await TagsClient.UpdateAtScopeAsync(resourceScope, tagPatchRequest);
                Assert.IsNull(patchResponse.Value.Properties.TagsValue);
            }
        }

        /// <summary>
        /// Patch request for Tags Operation within tracked resources test, including Replace|Merge|Delete operations
        /// </summary>
        [Test]
        public void UpdateTagsWithTrackedResourcesTest()
        {
            // test tags for tracked resources
            string resourceScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a/resourcegroups/TagsApiSDK/providers/Microsoft.Compute/virtualMachines/TagTestVM";
            UpdateTagsTest(resourceScope);
        }

        /// <summary>
        /// Patch request for Tags Operation within subscription test, including Replace|Merge|Delete operations
        /// </summary>
        [Test]
        public void UpdateTagsWithSubscriptionTest()
        {
            // test tags for subscription
            string subscriptionScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a";
            UpdateTagsTest(subscriptionScope);
        }

        /// <summary>
        /// Utility method to test Get request for Tags Operation within tracked resources and proxy resources
        /// </summary>
        private async void GetTagsTest(string resourceScope)
        {
            // using Tags.CreateOrUpdateAtScope to create two tags initially
            var tagsResource = new TagsResource(new Tags()
            {
                TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue1" },
                    { "tagKey2", "tagValue2" }
                }
            });
            await TagsClient.CreateOrUpdateAtScopeAsync(resourceScope, tagsResource.Properties);
            Thread.Sleep(3000);

            // get request should return created TagsResource
            var getResponse = await TagsClient.GetAtScopeAsync(resourceScope);
            Assert.AreEqual(getResponse.Value.Properties.TagsValue.Count(), tagsResource.Properties.TagsValue.Count());
            Assert.IsTrue(this.CompareTagsResource(tagsResource, getResponse));
            //getResponse.Properties.TagsProperty.Should().HaveCount(tagsResource.Properties.TagsProperty.Count);
            //this.CompareTagsResource(tagsResource, getResponse).Should().BeTrue();
        }

        /// <summary>
        /// Get request for Tags Operation within tracked resources test
        /// </summary>
        [Test]
        public void GetTagsWithTrackedResourcesTest()
        {
             // test tags for tracked resources
             string resourceScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a/resourcegroups/TagsApiSDK/providers/Microsoft.Compute/virtualMachines/TagTestVM";
             GetTagsTest(resourceScope);
        }

        /// <summary>
        /// Get request for Tags Operation within subscription test
        /// </summary>
        [Test]
        public void GetTagsWithSubscriptionTest()
        {
            // test tags for subscription
            string subscriptionScope = "/subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a";
            GetTagsTest(subscriptionScope);
        }

        /// <summary>
        /// Utility method to test Delete request for Tags Operation within tracked resources and proxy resources
        /// </summary>
        private async Task<TagsResource> DeleteTagsTest(string resourceScope)
        {
            // using Tags.CreateOrUpdateAtScope to create two tags initially
            var tagsResource = new TagsResource(new Tags()
            {
                TagsValue = new Dictionary<string, string> {
                    { "tagKey1", "tagValue1" },
                    { "tagKey2", "tagValue2" }
                }
            });
            await TagsClient.CreateOrUpdateAtScopeAsync(resourceScope, tagsResource.Properties);
            Thread.Sleep(3000);

            // try to delete existing tags
            await TagsClient.DeleteAtScopeAsync(resourceScope);
            Thread.Sleep(3000);
            // after deletion, Get request should get 0 tags back
            return await TagsClient.GetAtScopeAsync(resourceScope);
        }

        /// <summary>
        /// Get request for Tags Operation within tracked resources test
        /// </summary>
        [Test]
        public void DeleteTagsWithTrackedResourcesTest()
        {
            // test tags for tracked resources
            string resourceScope = "//subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a/resourcegroups/TagsApiSDK/providers/Microsoft.Compute/virtualMachines/TagTestVM";
            Assert.IsNull(DeleteTagsTest(resourceScope).Result.Properties.TagsValue);
        }

        /// <summary>
        /// Get request for Tags Operation within subscription test
        /// </summary>
        [Test]
        public void DeleteTagsWithSubscriptionTest()
        {
            // test tags for subscription
            string subscriptionScope = "//subscriptions/afe8f803-7190-48e3-b41a-bc454e8aad1a";
            Assert.IsNull(DeleteTagsTest(subscriptionScope).Result.Properties.TagsValue);
            //this.DeleteTagsTest(subscriptionScope).Properties.TagsProperty.Should().BeNull();
        }

        /// <summary>
        /// utility method to compare two TagsResource object to see if they are the same
        /// </summary>
        /// <param name="tag1">first TagsResource object</param>
        /// <param name="tag2">second TagsResource object</param>
        /// <returns> boolean to show whether two objects are the same</returns>
        private bool CompareTagsResource(TagsResource tag1, TagsResource tag2)
        {
            if ((tag1 == null && tag2 == null) || (tag1.Properties.TagsValue.Count == 0 && tag2.Properties.TagsValue.Count == 0))
            {
                return true;
            }
            if ((tag1 == null || tag2 == null) || (tag1.Properties.TagsValue.Count == 0 || tag2.Properties.TagsValue.Count == 0))
            {
                return false;
            }
            foreach (var pair in tag1.Properties.TagsValue)
            {
                if (!tag2.Properties.TagsValue.ContainsKey(pair.Key) || tag2.Properties.TagsValue[pair.Key] != pair.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
