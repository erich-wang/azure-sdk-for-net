// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.ResourceManager.TestFramework;

namespace Azure.ResourceManager.Storage.Tests.Helpers
{
    [ClientTestFixture]
    public abstract class StorageTestsManagementClientBase : ManagementRecordedTestBase<StorageManagementTestEnvironment>
    {
        public bool IsTestTenant = false;
        public static TimeSpan ZeroPollingInterval { get; } = TimeSpan.FromSeconds(0);
        public Dictionary<string, string> Tags { get; internal set; }

        public ResourcesManagementClient ResourceManagementClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public ResourceGroupsClient ResourceGroupsClient { get; set; }

        public StorageManagementClient StorageManagementClient { get; set; }
        public UsagesClient UsagesClient { get; set; }
        public StorageAccountsClient AccountsClient { get; set; }
        public SkusClient SkusClient { get; set; }
        public ManagementPoliciesClient ManagementPoliciesClient { get; set; }
        public PrivateEndpointConnectionsClient PrivateEndpointConnectionsClient { get; set; }
        public PrivateLinkResourcesClient PrivateLinkResourcesClient { get; set; }
        public EncryptionScopesClient EncryptionScopesClient { get; set; }
        public FileSharesClient FileSharesClient { get; set; }
        public FileServicesClient FileServicesClient { get; set; }
        public BlobServicesClient BlobServicesClient { get; set; }
        public BlobContainersClient BlobContainersClient { get; set; }
        public ObjectReplicationPoliciesClient ObjectReplicationPoliciesClient { get; set; }

        protected StorageTestsManagementClientBase(bool isAsync) : base(isAsync)
        {
        }

        protected void Initialize()
        {
            ResourceManagementClient = GetResourceManagementClient();
            ResourcesClient = ResourceManagementClient.GetResourcesClient();
            ResourceGroupsClient = ResourceManagementClient.GetResourceGroupsClient();

            StorageManagementClient = GetStorageManagementClient();
            UsagesClient = StorageManagementClient.GetUsagesClient();
            AccountsClient = StorageManagementClient.GetStorageAccountsClient();
            SkusClient = StorageManagementClient.GetSkusClient();
            ManagementPoliciesClient = StorageManagementClient.GetManagementPoliciesClient();
            PrivateEndpointConnectionsClient = StorageManagementClient.GetPrivateEndpointConnectionsClient();
            PrivateLinkResourcesClient = StorageManagementClient.GetPrivateLinkResourcesClient();
            EncryptionScopesClient = StorageManagementClient.GetEncryptionScopesClient();
            FileSharesClient = StorageManagementClient.GetFileSharesClient();
            FileServicesClient = StorageManagementClient.GetFileServicesClient();
            BlobServicesClient = StorageManagementClient.GetBlobServicesClient();
            BlobContainersClient = StorageManagementClient.GetBlobContainersClient();
            ObjectReplicationPoliciesClient = StorageManagementClient.GetObjectReplicationPoliciesClient();
        }

        private StorageManagementClient GetStorageManagementClient()
        {
            return CreateClient<StorageManagementClient>(new StorageManagementClient(TestEnvironment.SubscriptionId,
                 TestEnvironment.Credential,
                 Recording.InstrumentClientOptions(new StorageManagementClientOptions())));
        }
    }
}
