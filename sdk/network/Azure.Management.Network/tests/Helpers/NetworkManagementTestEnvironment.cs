// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.TestFramework;
//using Azure.Core.Testing;

namespace Azure.Management.Network.Tests.Helpers
{
    public class NetworkManagementTestEnvironment : TestEnvironment
    {
        public NetworkManagementTestEnvironment() : base("networkmgmt")
        {
        }
    }
}
