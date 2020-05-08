﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.Resource;
using Azure.Management.Resource.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;


namespace Azure.Management.EventHub.Tests
{
    [ClientTestFixture(EventHubManagementClientOption.ServiceVersion.V2019_09_01)]
    [NonParallelizable]
    public abstract class EventHubManagementClientBase : RecordedTestBase<EventhubTestEnvironment>
    {
        private readonly EventHubManagementClientOption.ServiceVersion _serviceVersion;
        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";

        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string subscriptionId { get; set; }

        //public AccessPolicyEntry accPol { get; internal set; }
        public string objectIdGuid { get; internal set; }
        public string rgName { get; internal set; }
        public string nsName { get; internal set; }
        public Dictionary<string, string> tags { get; internal set; }
        public Guid tenantIdGuid { get; internal set; }

        public OperationsClient OperationsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public ProvidersClient ResourceProvidersClient { get; set; }
        public EventHubsClient EventHubsClient { get; set; }
        public NamespacesClient NamespacesClient { get; set; }
        public ConsumerGroupsClient ConsumerGroupsClient { get; set; }
        public DisasterRecoveryConfigsClient DisasterRecoveryConfigsClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        protected EventHubManagementClientBase(bool isAsync,EventHubManagementClientOption.ServiceVersion serviceVersion)
            : base(isAsync)
        {
            _serviceVersion = serviceVersion;
        }

        protected  void  InitializeClients()
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
            EventHubsClient = GetHubsClient();
            NamespacesClient = GetNamespacesClient();
            ConsumerGroupsClient = GetConsumerGroupsClient();
            ResourcesClient = GetResourcesClient();
            //GraphUsersClient = GetGraphUsersClient();
            ResourceProvidersClient = GetResourceProvidersClient();
            DisasterRecoveryConfigsClient =GetDisasterRecoveryConfigsClient();
            ResourceGroupsClient = GetResourceGroupClient();
            OperationsClient = GetOperationsClient();
            //if (Mode == RecordedTestMode.Playback)
            //{
            //    this.objectId = Recording.GetVariable(ObjectIdKey, string.Empty);
            //}
            //else if (Mode == RecordedTestMode.Record)
            //{
            //    //TODO: verify in record mode; seems graph request is not in records, why?
            //    var userName = TestEnvironment.UserName;
            //    //this.objectId = (await GraphUsersClient.GetAsync(userName)).Value.ObjectId;
            //}
            //location = await GetLocation();
            //rgName = Recording.GenerateAssetName("sdktestrg");
            //nsName = Recording.GenerateAssetName(Helper.NamespacePrefix);
            //vaultName = Recording.GenerateAssetName("sdktestvault");
            //tenantIdGuid = new Guid(tenantId);
            //objectIdGuid = objectId;
            //tags = new Dictionary<string, string> { { "tag1", "value1" }, { "tag2", "value2" }, { "tag3", "value3" } };
        }

        public async Task<string> GetLocation()
        {
            var provider = (await ResourceProvidersClient.GetAsync("Microsoft.EventHub")).Value;
            this.location = provider.ResourceTypes.Where(
                (resType) =>
                {
                    if (resType.ResourceType == "namespaces")
                        return true;
                    else
                        return false;
                }
                ).First().Locations.FirstOrDefault();
            return location;
        }
        //public static Task TryRegisterResourceGroupAsync(ResourceGroupsClient resourceGroupsClient, string location, string resourceGroupName)
        //{
        //    resourceGroupsClient.CreateOrUpdate(resourceGroupName, new ResourceGroup(location));
        //}

        public static string TryGetResourceGroup (ResourceGroupsClient resourceGroupsClient, string location)
        {
            //AsyncPageable<Resource.Models.ResourceGroup> resourceGroup = resourceGroupsClient.ListAsync();
            var resourceGroup = resourceGroupsClient.ListAsync();
            var resourceGroupResult = resourceGroup.ToEnumerableAsync().Result.Where(group => string.IsNullOrWhiteSpace(location) || group.Location.Equals(location.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(group => group.Name.Contains(""));
            //var resourceGroupResult = resourceGroup.Where(group => string.IsNullOrWhiteSpace(location) || group.Location.Equals(location.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase))
            //    .FirstOrDefault(group => group.Name.Contains(""));
            return resourceGroupResult != null
                ? resourceGroupResult.Name
                : string.Empty;
        }

        internal EventHubsClient GetHubsClient(TestRecording recording=null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new EventHubsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new EventHubManagementClientOption(_serviceVersion))));
        }

        internal NamespacesClient GetNamespacesClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new NamespacesClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new EventHubManagementClientOption(_serviceVersion))));
        }

        internal ResourcesClient GetResourcesClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ResourcesClient(this.subscriptionId,
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

        internal ConsumerGroupsClient GetConsumerGroupsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ConsumerGroupsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new EventHubManagementClientOption(_serviceVersion))));
        }

        internal DisasterRecoveryConfigsClient GetDisasterRecoveryConfigsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;
            //ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId
            return InstrumentClient(new DisasterRecoveryConfigsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new EventHubManagementClientOption(_serviceVersion))));
        }
        internal OperationsClient GetOperationsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;
            //ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId
            return InstrumentClient(new OperationsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new EventHubManagementClientOption(_serviceVersion))));
        }
        internal ResourceGroupsClient GetResourceGroupClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;
            //ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId
            return InstrumentClient(new ResourceGroupsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }
        public override void StartTestRecording()
        {
            base.StartTestRecording();
        }

        public void isDelay(int t)
        {
            if (Mode == RecordedTestMode.Record /*|| Mode == RecordedTestMode.Playback*/)
            {
                Task.Delay(t*1000);
                //ChallengeBasedAuthenticationPolicy.AuthenticationChallenge.ClearCache();
            }
        }
    }
}
