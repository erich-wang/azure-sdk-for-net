// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.TestFramework;
using Azure.ResourceManager.Resource.Tests;
using Azure.ResourceManager.TestFramework;

namespace Azure.ResourceManager.Resources.Tests
{
    [RunFrequency(RunTestFrequency.Manually)]
    public abstract class ResourceOperationsTestsBase : ManagementRecordedTestBase<ResourceManagementTestEnvironment>
    {
        public static TimeSpan ZeroPollingInterval { get; } = TimeSpan.FromSeconds(0);
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

        internal ResourcesManagementClient GetResourcesManagementClient()
        {
            var options = Recording.InstrumentClientOptions(new ResourcesManagementClientOptions());
            CleanupPolicy = new ResourceGroupCleanupPolicy();
            options.AddPolicy(CleanupPolicy, HttpPipelinePosition.PerCall);

            return CreateClient<ResourcesManagementClient>(
                TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                options);
        }
    }
}
