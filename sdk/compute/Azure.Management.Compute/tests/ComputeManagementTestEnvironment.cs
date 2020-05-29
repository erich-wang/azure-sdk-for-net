// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.TestFramework;

namespace Azure.Management.Compute.Tests
{
    public class ComputeManagementTestEnvironment : TestEnvironment
    {
        private const string SubIdKey = "SubscriptionId";
        public ComputeManagementTestEnvironment() : base("computemgmt")
        {
        }

        public string UserName => GetVariable("AZURE_USER_NAME");

        public string SubscriptionIdTrack1 => GetRecordedVariable(SubIdKey);
    }
}
