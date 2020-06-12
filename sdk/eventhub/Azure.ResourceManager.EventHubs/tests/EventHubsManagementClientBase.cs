// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.ResourceManager.TestFramework;
using NUnit.Framework;

namespace Azure.ResourceManager.EventHubs.Tests
{
    [RunFrequency(RunTestFrequency.Manually)]
    [NonParallelizable]
    public abstract class EventHubsManagementClientBase : ManagementRecordedTestBase<EventHubsManagementTestEnvironment>
    {
        private const string ApplicationIdKey = "ApplicationId";
        public static TimeSpan ZeroPollingInterval { get; } = TimeSpan.FromSeconds(0);
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string Location { get; set; }
        public string SubscriptionId { get; set; }
        public ResourcesManagementClient ResourcesManagementClient { get; set; }
        public EventHubsManagementClient EventHubsManagementClient { get; set; }
        public OperationsClient OperationsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public ProvidersClient ResourceProvidersClient { get; set; }
        public EventHubsClient EventHubsClient { get; set; }
        public NamespacesClient NamespacesClient { get; set; }
        public ConsumerGroupsClient ConsumerGroupsClient { get; set; }
        public DisasterRecoveryConfigsClient DisasterRecoveryConfigsClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        protected EventHubsManagementClientBase(bool isAsync)
             : base(isAsync)
        {
        }

        protected void InitializeClients()
        {
            this.TenantId = TestEnvironment.TenantId;
            this.SubscriptionId = TestEnvironment.SubscriptionId;
            ResourcesManagementClient = GetResourceManagementClient();
            ResourcesClient = ResourcesManagementClient.GetResourcesClient();
            ResourceProvidersClient = ResourcesManagementClient.GetProvidersClient();
            ResourceGroupsClient = ResourcesManagementClient.GetResourceGroupsClient();

            EventHubsManagementClient = GetEventHubManagementClient();
            EventHubsClient = EventHubsManagementClient.GetEventHubsClient();
            NamespacesClient = EventHubsManagementClient.GetNamespacesClient();
            ConsumerGroupsClient = EventHubsManagementClient.GetConsumerGroupsClient();
            DisasterRecoveryConfigsClient = EventHubsManagementClient.GetDisasterRecoveryConfigsClient();
            OperationsClient = EventHubsManagementClient.GetOperationsClient();
        }

        internal EventHubsManagementClient GetEventHubManagementClient()
        {
            return CreateClient<EventHubsManagementClient>(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new EventHubsManagementClientOptions()));
        }

        public async Task<string> GetLocation()
        {
            var provider = (await ResourceProvidersClient.GetAsync("Microsoft.EventHub")).Value;
            this.Location = provider.ResourceTypes.Where(
                (resType) =>
                {
                    if (resType.ResourceType == "namespaces")
                        return true;
                    else
                        return false;
                }
                ).First().Locations.FirstOrDefault();
            return Location;
        }

        public void IsDelay(int t)
        {
            if (Mode == RecordedTestMode.Record)
            {
                Task.Delay(t * 1000);
            }
        }
    }
}
