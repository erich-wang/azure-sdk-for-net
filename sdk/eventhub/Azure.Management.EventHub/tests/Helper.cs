// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Management.Resource;
using Azure.Management.Resource.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace Azure.Management.EventHub.Tests
{
    public static class Helper
    {
        internal const string ResourceGroupPrefix = "Default-EventHub-";
        internal const string NamespacePrefix = "sdk-Namespace-";
        internal const string AuthorizationRulesPrefix = "sdk-Authrules-";
        internal const string DefaultNamespaceAuthorizationRule = "RootManageSharedAccessKey";
        internal const string EventHubPrefix = "sdk-EventHub-";
        internal const string ConsumerGroupPrefix = "sdk-ConsumerGroup-";
        internal const string DisasterRecoveryPrefix = "sdk-DisasterRecovery-";

        public static string ConvertObjectToJSon<T>(T obj)
        {
            return ConvertObjectToJSonAsync(obj);
        }

        public static string ConvertObjectToJSonAsync(object obj)
        {
            if (obj != null)
            {
                return (Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj, SerializeMediaTypeFormatterSettings))).Result;
            }
            return String.Empty;
        }
        public static string GenerateRandomKey()
        {
            byte[] key256 = new byte[32];
            using (var rngCryptoServiceProvider = RandomNumberGenerator.Create())
            {
                rngCryptoServiceProvider.GetBytes(key256);
            }

            return Convert.ToBase64String(key256);
        }
        public static async Task TryRegisterResourceGroupAsync(ResourceGroupsClient resourceGroupsClient, string location, string resourceGroupName)
        {
            await resourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
        }

        private static readonly JsonSerializerSettings SerializeMediaTypeFormatterSettings = new JsonSerializerSettings
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter { CamelCaseText = false },
            },
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };
    }
}
