// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using NUnit.Framework;

namespace Azure.Management.EventHub.Tests
{
    [NonParallelizable]
    public abstract class EventHubManagementClientBase : RecordedTestBase<EventhubTestEnvironment>
    {
        private const string ApplicationIdKey = "ApplicationId";
        public static TimeSpan ZeroPollingInterval { get; } = TimeSpan.FromSeconds(0);
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string Location { get; set; }
        public string SubscriptionId { get; set; }
        public ResourcesManagementClient ResourcesManagementClient { get; set; }
        public EventHubManagementClient EventHubManagementClient { get; set; }
        public OperationsClient OperationsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public ProvidersClient ResourceProvidersClient { get; set; }
        public EventHubsClient EventHubsClient { get; set; }
        public NamespacesClient NamespacesClient { get; set; }
        public ConsumerGroupsClient ConsumerGroupsClient { get; set; }
        public DisasterRecoveryConfigsClient DisasterRecoveryConfigsClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        protected EventHubManagementClientBase(bool isAsync)
             : base(isAsync)
        {
        }

        protected void InitializeClients()
        {
            this.TenantId = TestEnvironment.TenantId;
            this.SubscriptionId = TestEnvironment.SubscriptionId;
            ResourcesManagementClient = GetResourcesManagementClient();
            ResourcesClient = ResourcesManagementClient.GetResourcesClient();
            ResourceProvidersClient = ResourcesManagementClient.GetProvidersClient();
            ResourceGroupsClient = ResourcesManagementClient.GetResourceGroupsClient();

            EventHubManagementClient = GetEventHubManagementClient();
            EventHubsClient = EventHubManagementClient.GetEventHubsClient();
            NamespacesClient = EventHubManagementClient.GetNamespacesClient();
            ConsumerGroupsClient = EventHubManagementClient.GetConsumerGroupsClient();
            DisasterRecoveryConfigsClient = EventHubManagementClient.GetDisasterRecoveryConfigsClient();
            OperationsClient = EventHubManagementClient.GetOperationsClient();
        }

        internal ResourcesManagementClient GetResourcesManagementClient()
        {
            return InstrumentClient(new ResourcesManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ResourcesManagementClientOptions())));
        }

        internal EventHubManagementClient GetEventHubManagementClient()
        {
            return InstrumentClient(new EventHubManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new EventHubManagementClientOptions())));
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
