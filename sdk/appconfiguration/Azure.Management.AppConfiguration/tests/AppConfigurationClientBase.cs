// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Core.TestFramework;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.AppConfiguration.Tests
{
    [NonParallelizable]
    public abstract class AppConfigurationClientBase : RecordedTestBase<AppConfigurationEnvironment>
    {
        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";
        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string subscriptionId { get; set; }
        public AppConfigurationManagementClient AppConfigurationManagementClient { get; set; }
        public ResourcesManagementClient ResourcesManagementClient { get; set; }
        public ConfigurationStoresClient ConfigurationStoresClient { get; set; }
        public PrivateEndpointConnectionsClient PrivateEndpointConnectionsClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        public PrivateLinkResourcesClient PrivateLinkResourcesClient { get; set; }
        public OperationsClient OperationsClient { get; set; }
        public string AZURE_LOCATION { get; set; }
        public string KEY_UUID { get; set; }
        public string LABEL_UUID { get; set; }
        public string KEY { get; set; }
        public string LABEL { get; set; }
        public string TEST_CONTENT_TYPE { get; set; }
        public string TEST_VALUE { get; set; }
        public string ResourceGroupPrefix { get; set; }
        protected AppConfigurationClientBase(bool isAsync)
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
            this.applicationId = Recording.GetVariable(ApplicationIdKey, Guid.NewGuid().ToString());
            ;
            AZURE_LOCATION = "eastus";
            KEY_UUID = "test_key_a6af8952-54a6-11e9-b600-2816a84d0309";
            LABEL_UUID = "1d7b2b28-549e-11e9-b51c-2816a84d0309";
            KEY = "PYTHON_UNIT_" + KEY_UUID;
            LABEL = "test_label1_" + LABEL_UUID;
            TEST_CONTENT_TYPE = "test content type";
            TEST_VALUE = "test value";
            ResourceGroupPrefix = "Default-EventHub-";
            AppConfigurationManagementClient = GetAppConfigurationManagementClient();
            ConfigurationStoresClient = AppConfigurationManagementClient.GetConfigurationStoresClient();
            PrivateEndpointConnectionsClient = AppConfigurationManagementClient.GetPrivateEndpointConnectionsClient();
            PrivateLinkResourcesClient = AppConfigurationManagementClient.GetPrivateLinkResourcesClient();
            OperationsClient = AppConfigurationManagementClient.GetOperationsClient();
            ResourcesManagementClient = GetResourceManagementClient();
            ResourceGroupsClient = ResourcesManagementClient.GetResourceGroupsClient();
        }

        internal AppConfigurationManagementClient GetAppConfigurationManagementClient()
        {
            return InstrumentClient(new AppConfigurationManagementClient(this.subscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new AppConfigurationManagementClientOptions())));
        }
        internal ResourcesManagementClient GetResourceManagementClient()
        {
            return InstrumentClient(new ResourcesManagementClient(this.subscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ResourcesManagementClientOptions())));
        }


        public static string TryGetResourceGroup(ResourceGroupsClient resourceGroupsClient, string location)
        {
            var resourceGroup = resourceGroupsClient.ListAsync();
            var resourceGroupResult = resourceGroup.ToEnumerableAsync().Result.Where(group => string.IsNullOrWhiteSpace(location) || group.Location.Equals(location.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(group => group.Name.Contains(""));
            return resourceGroupResult != null
                ? resourceGroupResult.Name
                : string.Empty;
        }
    }
}
