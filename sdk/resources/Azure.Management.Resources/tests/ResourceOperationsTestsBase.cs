// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Core.TestFramework;
using Azure.Management.Resource;
using Azure.Management.Resource.Tests;

using NUnit.Framework;

namespace Azure.Management.Resources.Tests
{
    [NonParallelizable]
    public abstract class ResourceOperationsTestsBase : RecordedTestBase<ResourceManagementTestEnvironment>
    {
        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";

        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string subscriptionId { get; set; }

        public ResourcesManagementClient ResourcesManagementClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        public DeploymentsClient DeploymentsClient { get; set; }
        public DeploymentScriptsClient DeploymentScriptsClient { get; set; }
        public DeploymentClient DeploymentClient { get; set; }
        public OperationsClient OperationsClient { get; set; }
        public ProvidersClient ProvidersClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public TagsClient TagsClient { get; set; }
        public SubscriptionsClient SubscriptionsClient { get; set; }
        public TenantsClient TenantsClient { get; set; }
        public PolicyAssignmentsClient PolicyAssignmentsClient { get; set; }
        public PolicyDefinitionsClient PolicyDefinitionsClient { get; set; }
        public PolicySetDefinitionsClient PolicySetDefinitionsClient { get; set; }


        protected ResourceOperationsTestsBase(bool isAsync)
            : base(isAsync)
        {
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

            ResourcesManagementClient = GetResourcesManagementClient();
            ResourceGroupsClient = ResourcesManagementClient.GetResourceGroupsClient();
            DeploymentsClient = ResourcesManagementClient.GetDeploymentsClient();
            DeploymentScriptsClient = ResourcesManagementClient.GetDeploymentScriptsClient();
            DeploymentClient = ResourcesManagementClient.GetDeploymentClient();
            OperationsClient = ResourcesManagementClient.GetOperationsClient();
            ProvidersClient = ResourcesManagementClient.GetProvidersClient();
            ResourcesClient = ResourcesManagementClient.GetResourcesClient();
            TagsClient = ResourcesManagementClient.GetTagsClient();
            SubscriptionsClient = ResourcesManagementClient.GetSubscriptionsClient();
            TenantsClient = ResourcesManagementClient.GetTenantsClient();
            PolicyAssignmentsClient = ResourcesManagementClient.GetPolicyAssignmentsClient();
            PolicyDefinitionsClient = ResourcesManagementClient.GetPolicyDefinitionsClient();
            PolicySetDefinitionsClient = ResourcesManagementClient.GetPolicySetDefinitionsClient();
        }

        internal ResourcesManagementClient GetResourcesManagementClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            var options = new ResourcesManagementClientOptions();
            CleanupPolicy = new ResCleanupPolicy();
            options.AddPolicy(CleanupPolicy, HttpPipelinePosition.PerCall);

            return InstrumentClient(new ResourcesManagementClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(options)));
        }

        private ResCleanupPolicy CleanupPolicy { get; set; }

        [TearDown]
        public async Task Cleanup()
        {
            if (Recording.Mode == RecordedTestMode.Record || Recording.Mode == RecordedTestMode.Live)
            {
                foreach (var resGroup in CleanupPolicy.ResourceGroupsCreated)
                {
                    var resourceGroupClient = new ResourcesManagementClient(TestEnvironment.SubscriptionId, TestEnvironment.Credential).GetResourceGroupsClient();
                    var operation = await resourceGroupClient.StartDeleteAsync(resGroup);
                    //need to wait for completion?
                    await operation.WaitForCompletionAsync();
                }
            }
        }

        public class ResCleanupPolicy : HttpPipelineSynchronousPolicy
        {
            private readonly IList<string> _resourceGroupCreated = new List<string>();
            private Regex _resourceGroupPattern = new Regex(@"/subscriptions/[^/]+/resourcegroups/([^?/]+)\?api-version");
            public IList<string> ResourceGroupsCreated
            {
                get { return _resourceGroupCreated; }
            }


            public override void OnSendingRequest(HttpMessage message)
            {
                if (message.Request.Method == RequestMethod.Put)
                {
                    var match = _resourceGroupPattern.Match(message.Request.Uri.ToString());
                    if (match.Success)
                    {
                        _resourceGroupCreated.Add(match.Groups[1].Value);
                    }
                }
            }
        }
    }
}
