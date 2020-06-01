// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Azure.Core.TestFramework;
using System.Reflection;
using System.Threading;
using Azure.Management.AppConfiguration.Tests;
using Azure.Management.AppConfiguration;
using Azure.Management.AppConfiguration.Models;
using Sku = Azure.Management.AppConfiguration.Models.Sku;
using System.Text.Json;

namespace Azure.Management.AppConfiguration.Tests
{
    public class AppConfigurationTest : AppConfigurationClientBase
    {
        public AppConfigurationTest(bool isAsync)
            : base(isAsync)
        {
        }
        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        public async Task Test_AppConfiguration_list_Key_Values()
        {

            var resourceGroup = TryGetResourceGroup(ResourceGroupsClient, AZURE_LOCATION);
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                resourceGroup = Recording.GenerateAssetName(ResourceGroupPrefix);
                await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient, AZURE_LOCATION, resourceGroup);
            }
            //create configuration
            var CONFIGURATION_STORE_NAME = Recording.GenerateAssetName("configuration");
            var configurationCreateResult = await ConfigurationStoresClient.StartCreateAsync(resourceGroup, CONFIGURATION_STORE_NAME, new ConfigurationStore("westus", new Sku("Standard")));
            var configCreate = (await configurationCreateResult.WaitForCompletionAsync()).Value;

            //list configuration
            var configListResult = ConfigurationStoresClient.ListKeysAsync(resourceGroup, CONFIGURATION_STORE_NAME);
            var conList = await configListResult.ToEnumerableAsync();
            //# ConfigurationStores_ListKeys[post]
            //    keys = list(self.mgmt_client.configuration_stores.list_keys(resource_group.name, CONFIGURATION_STORE_NAME))
            var configRegenerateResult = ConfigurationStoresClient.RegenerateKeyAsync(resourceGroup, CONFIGURATION_STORE_NAME,new RegenerateKeyParameters(conList.First().Id));

            //if self.is_live:
            //    # create key-value
            //    self.create_kv(key.connection_string)

            //create Key
            //var addConfigurationsetting = await PrivateEndpointConnectionsClient
            //[skip]
            //var listkeyvalueResult = await ConfigurationStoresClient.ListKeyValueAsync(resourceGroup, CONFIGURATION_STORE_NAME,KEY,LABEL);
            //# ConfigurationStores_ListKeyValue[post]
            //    BODY = {
            //        "key": KEY,
            //  "label": LABEL
            //}
            //    result = self.mgmt_client.configuration_stores.list_key_value(resource_group.name, CONFIGURATION_STORE_NAME, BODY)
        }

        [Test]
        public async Task Test_AppConfiguration()
        {
            //string SERVICE_NAME = "myapimrndxyz";
            //string VNET_NAME = "vnetname";
            //string SUB_NET = "subnetname";
            //string ENDPOINT_NAME = "endpointxyz";D:\sdk\20200507\azure-sdk-for-net\sdk\appconfiguration\Azure.Management.AppConfiguration\src\Generated\Operations\PrivateEndpointConnectionsRestClient.cs
            string CONFIGURATION_STORE_NAME = Recording.GenerateAssetName("configuration");
            string PRIVATE_ENDPOINT_CONNECTION_NAME = Recording.GenerateAssetName("privateendpoint");
            var resourceGroup = TryGetResourceGroup(ResourceGroupsClient, AZURE_LOCATION);
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                resourceGroup = Recording.GenerateAssetName(ResourceGroupPrefix);
                await Helper.TryRegisterResourceGroupAsync(ResourceGroupsClient, AZURE_LOCATION, resourceGroup);
            }
            //JsonElement jsonElement= new JsonElement() { }
            //DeserializeConfigurationStore
            var configurationCreateResult = await ConfigurationStoresClient.StartCreateAsync(resourceGroup, CONFIGURATION_STORE_NAME, new ConfigurationStore("westus", new Sku("Standard")));
            var configCreate = (await configurationCreateResult.WaitForCompletionAsync()).Value;

            //if (Mode == RecordedTestMode.Record)
            //{
            //   await PrivateEndpointConnectionsClient.StartCreateOrUpdateAsync(resourceGroup, CONFIGURATION_STORE_NAME, SUB_NET, ENDPOINT_NAME, configCreate.Id);
            //   string resourceGroupName, string configStoreName, string privateEndpointConnectionName, PrivateEndpoint privateEndpoint = null, PrivateLinkServiceConnectionState privateLinkServiceConnectionState = null, CancellationToken cancellationToken = default
            //}
            var configurationGetResult = await ConfigurationStoresClient.GetAsync(resourceGroup, CONFIGURATION_STORE_NAME);
            //PRIVATE_ENDPOINT_CONNECTION_NAME = configurationGetResult.Value.Endpoint;
            //private_connection_id = conf_store.private_endpoint_connections[0].id
            //            BODY = {
            //# "id": "https://management.azure.com/subscriptions/" + self.settings.SUBSCRIPTION_ID + "/resourceGroups/" + resource_group.name + "/providers/Microsoft.AppConfiguration/configurationStores/" + CONFIGURATION_STORE_NAME + "/privateEndpointConnections/" + PRIVATE_ENDPOINT_CONNECTION_NAME,
            //                "id": private_connection_id,
            //          "private_endpoint": {
            //                    "id": "/subscriptions/" + self.settings.SUBSCRIPTION_ID + "/resourceGroups/" + resource_group.name + "/providers/Microsoft.Network/privateEndpoints/" + ENDPOINT_NAME,
            //          },
            //          "private_link_service_connection_state": {
            //                    "status": "Approved",
            //            "description": "Auto-Approved"
            //          }
            //            }
            //            result = self.mgmt_client.private_endpoint_connections.begin_create_or_update(
            //                resource_group.name,
            //                CONFIGURATION_STORE_NAME,
            //                PRIVATE_ENDPOINT_CONNECTION_NAME,
            //                BODY)
            //            # id=BODY["id"],
            //            # private_endpoint=BODY["private_endpoint"],
            //            # private_link_service_connection_state=BODY["private_link_service_connection_state"])
            //        result = result.result()
            //[Skip]
            //var private_endpoint_getResult = await PrivateEndpointConnectionsClient.GetAsync(resourceGroup, CONFIGURATION_STORE_NAME, PRIVATE_ENDPOINT_CONNECTION_NAME);

            var list_by_configuration_Result = PrivateLinkResourcesClient.ListByConfigurationStoreAsync(resourceGroup, CONFIGURATION_STORE_NAME);

            var list_by_configuration_Res = await list_by_configuration_Result.ToEnumerableAsync();
            var PRIVATE_LINK_RESOURCE_NAME = list_by_configuration_Res.First().Name;
            //[Skip]
            //var private_link_resource_getResult = await PrivateLinkResourcesClient.GetAsync(resourceGroup, CONFIGURATION_STORE_NAME, PRIVATE_LINK_RESOURCE_NAME);

            var list_by_configuration_storeResult = PrivateEndpointConnectionsClient.ListByConfigurationStoreAsync(resourceGroup, CONFIGURATION_STORE_NAME);
            var list_by_configuration_storeRe = await list_by_configuration_storeResult.ToEnumerableAsync();
            //# PrivateEndpointConnection_List[get]
            //    result = list(self.mgmt_client.private_endpoint_connections.list_by_configuration_store(resource_group.name, CONFIGURATION_STORE_NAME))

            var operationListResult = OperationsClient.ListAsync();

            var configuration_store_list_by_resource_groupResult = ConfigurationStoresClient.ListByResourceGroupAsync(resourceGroup);

            var configuration_stores_list_Result = ConfigurationStoresClient.ListAsync(resourceGroup);

            var configuration_stores_begin_updateResult = ConfigurationStoresClient.StartUpdateAsync(resourceGroup, CONFIGURATION_STORE_NAME, new ConfigurationStoreUpdateParameters()
            {
                Tags = new Dictionary<string, string> { { "category", "Marketing" } },
                Sku = new Sku("Standard")
            });
        }
    }
}
