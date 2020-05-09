using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Graph.Rbac;
using Azure.Graph.Rbac.Models;
using Azure.Identity;
using Azure.Management.Resource.Models;
using Azure.Management.Resource;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Azure.Management.Resource.Tests
{
    [ClientTestFixture(ResourceManagementClientOptions.ServiceVersion.V2019_10_01)]
    [NonParallelizable]
    public abstract class ResourceOperationsTestsBase : RecordedTestBase<ResourceManagementTestEnvironment>
    {
        private readonly ResourceManagementClientOptions.ServiceVersion _serviceVersion;

        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";

        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string subscriptionId { get; set; }

        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        public ProvidersClient ResourceProvidersClient { get; set; }
        public DeploymentsClient DeploymentsClient { get; set; }
        public DeploymentScriptsClient DeploymentScriptsClient { get; set; }
        public TagsClient TagsClient{ get; set; }



        protected ResourceOperationsTestsBase(bool isAsync, ResourceManagementClientOptions.ServiceVersion serviceVersion)
            : base(isAsync)
        {
            _serviceVersion = serviceVersion;
        }

        protected void Initialize()
        {
            if (Mode == RecordedTestMode.Playback && Recording.IsTrack1SessionRecord())
            {
                this.tenantId = TestEnvironment.TenantIdTrack1;
                this.subscriptionId = TestEnvironment.SubscriptionIdTrack1;
            }
            else
            {
                this.tenantId = TestEnvironment.TenantId;
                this.subscriptionId = TestEnvironment.SubscriptionId;
            }

            ResourceGroupsClient = GetResourceGroupsClient();
            ResourceProvidersClient = GetResourceProvidersClient();
            DeploymentsClient = GetDeploymentClient();
            DeploymentScriptsClient = GetDeploymentScriptsClient();
            TagsClient = GetTagsClient();
        }

        internal ResourceGroupsClient GetResourceGroupsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ResourceGroupsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        internal ProvidersClient GetResourceProvidersClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ProvidersClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        internal DeploymentsClient GetDeploymentClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new DeploymentsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        internal DeploymentScriptsClient GetDeploymentScriptsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new DeploymentScriptsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        internal TagsClient GetTagsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new TagsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }
    }
 }
