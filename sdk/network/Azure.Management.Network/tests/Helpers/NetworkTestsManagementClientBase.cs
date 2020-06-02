// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Azure.Core.TestFramework;
using Azure.Management.Compute;
using Azure.Management.Resources;
using Azure.Management.Storage;

namespace Azure.Management.Network.Tests.Helpers
{
    public class NetworkTestsManagementClientBase : RecordedTestBase<NetworkManagementTestEnvironment>
    {
        public bool IsTestTenant = false;
        public Dictionary<string, string> Tags { get; internal set; }

        public ResourcesManagementClient ResourceManagementClient { get; set; }
        public StorageManagementClient StorageManagementClient { get; set; }
        public ComputeManagementClient ComputeManagementClient { get; set; }
        public NetworkManagementClient NetworkManagementClient { get; set; }

        public NetworkInterfacesClient NetworkInterfacesClient { get; set; }
        public ProvidersClient ProvidersClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public ServiceClient ServiceClient { get; set; }

        protected NetworkTestsManagementClientBase(bool isAsync) : base(isAsync)
        {
        }

        protected void Initialize()
        {
            ResourceManagementClient = GetResourceManagementClient();
            StorageManagementClient = GetStorageManagementClient();
            ComputeManagementClient = GetComputeManagementClient();
            NetworkManagementClient = GetNetworkManagementClient();

            NetworkInterfacesClient = NetworkManagementClient.GetNetworkInterfacesClient();
            ProvidersClient = ResourceManagementClient.GetProvidersClient();
            ResourceGroupsClient = ResourceManagementClient.GetResourceGroupsClient();
            ResourcesClient = ResourceManagementClient.GetResourcesClient();
            ServiceClient = NetworkManagementClient.GetServiceClient();
        }

        private ResourcesManagementClient GetResourceManagementClient()
        {
            return InstrumentClient(new ResourcesManagementClient(TestEnvironment.SubscriptionId,
                 TestEnvironment.Credential,
                 Recording.InstrumentClientOptions(new ResourcesManagementClientOptions())));
        }

        private StorageManagementClient GetStorageManagementClient()
        {
            return InstrumentClient(new StorageManagementClient(TestEnvironment.SubscriptionId,
                 TestEnvironment.Credential,
                 Recording.InstrumentClientOptions(new StorageManagementClientOptions())));
        }

        private ComputeManagementClient GetComputeManagementClient()
        {
            return InstrumentClient(new ComputeManagementClient(TestEnvironment.SubscriptionId,
                 TestEnvironment.Credential,
                 Recording.InstrumentClientOptions(new ComputeManagementClientOptions())));
        }

        private NetworkManagementClient GetNetworkManagementClient()
        {
            return InstrumentClient(new NetworkManagementClient(TestEnvironment.SubscriptionId,
                 TestEnvironment.Credential,
                 Recording.InstrumentClientOptions(new NetworkManagementClientOptions())));
        }

        public override void StartTestRecording()
        {
            base.StartTestRecording();
        }
    }
}
