// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class TroubleshootTests : NetworkTestsManagementClientBase
    {
        public TroubleshootTests(bool isAsync) : base(isAsync)
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
        [Ignore("The NetworkWathcer is involved, so disable the test")]
        public async Task TroubleshootApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            try
            {
                string location = "westus2";
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));

                // CreateVirtualNetworkGateway API
                // Prerequisite:- Create PublicIPAddress(Gateway Ip) using Put PublicIPAddress API
                string publicIpName = Recording.GenerateAssetName("azsmnet");
                string domainNameLabel = Recording.GenerateAssetName("azsmnet");

                PublicIPAddress nic1publicIp = await TestHelper.CreateDefaultPublicIpAddress(publicIpName, resourceGroupName, domainNameLabel, location, NetworkManagementClient);

                //Prerequisite:-Create Virtual Network using Put VirtualNetwork API
                string vnetName = Recording.GenerateAssetName("azsmnet");
                string subnetName = "GatewaySubnet";

                await TestHelper.CreateVirtualNetwork(vnetName, subnetName, resourceGroupName, location, NetworkManagementClient);

                Response<Subnet> getSubnetResponse = await NetworkManagementClient.GetSubnetsClient().GetAsync(resourceGroupName, vnetName, subnetName);

                // CreateVirtualNetworkGateway API
                string virtualNetworkGatewayName = Recording.GenerateAssetName("azsmnet");
                string ipConfigName = Recording.GenerateAssetName("azsmnet");

                VirtualNetworkGateway virtualNetworkGateway = new VirtualNetworkGateway()
                {
                    Location = location,
                    Tags = new Dictionary<string, string>() { { "key", "value" } },
                    EnableBgp = false,
                    GatewayDefaultSite = null,
                    GatewayType = VirtualNetworkGatewayType.Vpn,
                    VpnType = VpnType.RouteBased,
                    IpConfigurations = new List<VirtualNetworkGatewayIPConfiguration>()
                    {
                        new VirtualNetworkGatewayIPConfiguration()
                        {
                            Name = ipConfigName,
                            PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                            PublicIPAddress = new SubResource() { Id = nic1publicIp.Id }, Subnet = new SubResource() { Id = getSubnetResponse.Value.Id }
                        }
                    },
                    Sku = new VirtualNetworkGatewaySku() { Name = VirtualNetworkGatewaySkuName.Basic, Tier = VirtualNetworkGatewaySkuTier.Basic }
                };

                VirtualNetworkGatewaysCreateOrUpdateOperation putVirtualNetworkGatewayResponseOperation =
                    await NetworkManagementClient.GetVirtualNetworkGatewaysClient().StartCreateOrUpdateAsync(resourceGroupName, virtualNetworkGatewayName, virtualNetworkGateway);
                await putVirtualNetworkGatewayResponseOperation.WaitForCompletionAsync();
                // GetVirtualNetworkGateway API
                Response<VirtualNetworkGateway> getVirtualNetworkGatewayResponse =
                    await NetworkManagementClient.GetVirtualNetworkGatewaysClient().GetAsync(resourceGroupName, virtualNetworkGatewayName);

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync(resourceGroupName, networkWatcherName, properties);

                //Create storage
                string storageName = Recording.GenerateAssetName("azsmnet");
                var storageParameters = new StorageAccountCreateParameters(new Storage.Models.Sku(SkuName.StandardLRS), Kind.Storage, location);

                Operation<StorageAccount> accountOperation = await StorageManagementClient.GetStorageAccountsClient().StartCreateAsync(resourceGroupName, storageName, storageParameters);
                Response<StorageAccount> account = await accountOperation.WaitForCompletionAsync();
                TroubleshootingParameters parameters = new TroubleshootingParameters(getVirtualNetworkGatewayResponse.Value.Id, account.Value.Id, "https://nwtestdbdzq4xsvskrei6.blob.core.windows.net/vhds");

                //Get troubleshooting
                NetworkWatchersGetTroubleshootingOperation troubleshootOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetTroubleshootingAsync("NetworkWatcherRG", "NetworkWatcher_westus2", parameters);
                await troubleshootOperation.WaitForCompletionAsync();
                QueryTroubleshootingParameters qParameters = new QueryTroubleshootingParameters(getVirtualNetworkGatewayResponse.Value.Id);

                //Query last troubleshoot
                NetworkWatchersGetTroubleshootingResultOperation queryTroubleshootOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetTroubleshootingResultAsync("NetworkWatcherRG", "NetworkWatcher_westus2", qParameters);
                await queryTroubleshootOperation.WaitForCompletionAsync();
                //TODO: make verification once fixed for troubleshoot API deployed
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }
    }
}
