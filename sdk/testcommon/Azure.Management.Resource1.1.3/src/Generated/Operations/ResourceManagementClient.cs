// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using Azure.Core;
using Azure.Management.Resource;

namespace Azure.Management.Resource
{
    /// <summary> Resource service management client. </summary>
    public class ResourceManagementClient
    {
        private readonly ResourceManagementClientOptions _options;
        private readonly TokenCredential _tokenCredential;
        private readonly string _subscriptionId;

        /// <summary> Initializes a new instance of ResourceManagementClient for mocking. </summary>
        protected ResourceManagementClient()
        {
        }

        /// <summary> Initializes a new instance of ResourceManagementClient. </summary>
        public ResourceManagementClient(string subscriptionId, TokenCredential tokenCredential, ResourceManagementClientOptions options = null)
        {
            _options = options ?? new ResourceManagementClientOptions();
            _tokenCredential = tokenCredential;
            _subscriptionId = subscriptionId;
        }

        /// <summary> Creates a new instance of DeploymentsClient. </summary>
        public DeploymentsClient GetDeploymentsClient()
        {
            return new DeploymentsClient(_subscriptionId, _tokenCredential, _options);
        }

        /// <summary> Creates a new instance of ProvidersClient. </summary>
        public ProvidersClient GetProvidersClient()
        {
            return new ProvidersClient(_subscriptionId, _tokenCredential, _options);
        }

        /// <summary> Creates a new instance of ResourceGroupsClient. </summary>
        public ResourceGroupsClient GetResourceGroupsClient()
        {
            return new ResourceGroupsClient(_subscriptionId, _tokenCredential, _options);
        }

        /// <summary> Creates a new instance of ResourcesClient. </summary>
        public ResourcesClient GetResourcesClient()
        {
            return new ResourcesClient(_subscriptionId, _tokenCredential, _options);
        }

        /// <summary> Creates a new instance of TagsClient. </summary>
        public TagsClient GetTagsClient()
        {
            return new TagsClient(_subscriptionId, _tokenCredential, _options);
        }

        /// <summary> Creates a new instance of DeploymentClient. </summary>
        public DeploymentClient GetDeploymentClient()
        {
            return new DeploymentClient(_subscriptionId, _tokenCredential, _options);
        }
    }
}
