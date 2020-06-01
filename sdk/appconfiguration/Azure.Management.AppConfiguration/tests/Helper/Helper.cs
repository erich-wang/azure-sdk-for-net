// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace Azure.Management.AppConfiguration.Tests
{
    public static class Helper
    {
        public static async Task TryRegisterResourceGroupAsync(ResourceGroupsClient resourceGroupsClient, string location, string resourceGroupName)
        {
            await resourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
        }
    }
}
