// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.TestFramework;

namespace Azure.Management.AppConfiguration.Tests
{
    public class AppConfigurationEnvironment : TestEnvironment
    {
        //private const string TenantIdKey = "TenantId";
        private const string SubIdKey = "SubscriptionId";
        public AppConfigurationEnvironment() : base("AppConfigurationtmgmt")
        {
        }

        //Do not need to save to session record
        public string UserName => GetVariable("AZURE_USER_NAME");
        public string TenantIdTrack1 => "854d368f-1828-428f-8f3c-f2affa9b2f7d";
        public string SubscriptionIdTrack1 => GetRecordedVariable(SubIdKey);
    }
}
