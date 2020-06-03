// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;

namespace Azure.Management.AppConfiguration.Tests
{
    public abstract class AppConfigurationClientBase : RecordedTestBase<AppConfigurationManagementTestEnvironment>
    {
        public static TimeSpan ZeroPollingInterval { get; } = TimeSpan.FromSeconds(0);
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
            return InstrumentClient(new AppConfigurationManagementClient(this.TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new AppConfigurationManagementClientOptions())));
        }

        internal ResourcesManagementClient GetResourceManagementClient()
        {
            return InstrumentClient(new ResourcesManagementClient(this.TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ResourcesManagementClientOptions())));
        }

        protected ValueTask<Response<T>> WaitForCompletionAsync<T>(Operation<T> operation)
        {
            if (Mode == RecordedTestMode.Playback)
            {
                return operation.WaitForCompletionAsync(ZeroPollingInterval, default);
            }
            else
            {
                return operation.WaitForCompletionAsync();
            }
        }
    }
}
