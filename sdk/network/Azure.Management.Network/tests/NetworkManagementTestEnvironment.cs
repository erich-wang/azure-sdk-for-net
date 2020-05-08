// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.Testing;

namespace Azure.Management.Network.Tests
{
    public class NetworkManagementTestEnvironment : TestEnvironment
    {
        //private const string TenantIdKey = "TenantId";
        private const string SubIdKey = "SubscriptionId";
        //private const string ApplicationIdKey = "ApplicationId";

        public NetworkManagementTestEnvironment() : base("networkmgmt")
        {
        }

        public string UserName => GetVariable("AZURE_USER_NAME");

        public string TenantIdTrack1 => "854d368f-1828-428f-8f3c-f2affa9b2f7d";
        public string SubscriptionIdTrack1 => GetRecordedVariable(SubIdKey);
    }
}
