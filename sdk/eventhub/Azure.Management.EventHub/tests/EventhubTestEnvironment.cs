// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.TestFramework;
namespace Azure.Management.EventHub.Tests
{
    public class EventhubTestEnvironment :TestEnvironment
    {
        //private const string TenantIdKey = "TenantId";
        private const string SubIdKey = "SubscriptionId";
        private const string CreatePrimaryKeyKey = "wK5NJXkHBpCa0aphhnUqT0uBlf3D4LTiG/2s0BrGH+k=";
        private const string UpdatePrimaryKeyKey = "zZ1CpYzOmuBFwkkeNWwRchEpr8DA8JZsjGXDoLQjSoM=";
        public EventhubTestEnvironment() : base("Eventhubtmgmt")
        {
        }

        //Do not need to save to session record
        public string UserName => GetVariable("AZURE_USER_NAME");

        //public string TenantIdTrack1 => "72f988bf-86f1-41af-91ab-2d7cd011db47";
        public string TenantIdTrack1 => "854d368f-1828-428f-8f3c-f2affa9b2f7d";
        public string SubscriptionIdTrack1 => GetRecordedVariable(SubIdKey);

        public string UpdatePrimaryKey => GetVariable(UpdatePrimaryKeyKey);

        public string CreatePrimaryKey => GetVariable(CreatePrimaryKeyKey);
    }
}
