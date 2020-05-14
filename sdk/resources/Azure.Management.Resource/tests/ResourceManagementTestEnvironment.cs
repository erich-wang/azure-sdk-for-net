// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.Testing;

namespace Azure.Management.Resource.Tests
{
    public class ResourceManagementTestEnvironment : TestEnvironment
    {
        public ResourceManagementTestEnvironment() : base("resourcemgmt")
        {
        }
    }
}