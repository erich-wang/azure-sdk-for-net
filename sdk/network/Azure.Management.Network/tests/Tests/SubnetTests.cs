﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class SubnetTests : NetworkTestsManagementClientBase
    {
        public SubnetTests(bool isAsync) : base(isAsync)
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
        public async Task SubnetApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("csmrg");

            try
            {
                string location = await NetworkManagementTestUtilities.GetResourceLocation(ResourceManagementClient, "Microsoft.Network/virtualNetworks");
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
                string vnetName = Recording.GenerateAssetName("azsmnet");
                string subnet1Name = Recording.GenerateAssetName("azsmnet");
                string subnet2Name = Recording.GenerateAssetName("azsmnet");

                VirtualNetwork vnet = new VirtualNetwork()
                {
                    Location = location,
                    AddressSpace = new AddressSpace()
                    {
                        AddressPrefixes = new List<string>() { "10.0.0.0/16", }
                    },
                    DhcpOptions = new DhcpOptions()
                    {
                        DnsServers = new List<string>() { "10.1.1.1", "10.1.2.4" }
                    },
                    Subnets = new List<Subnet>() { new Subnet() { Name = subnet1Name, AddressPrefix = "10.0.0.0/24", } }
                };

                VirtualNetworksCreateOrUpdateOperation putVnetResponseOperation = await NetworkManagementClient.GetVirtualNetworksClient().StartCreateOrUpdateAsync(resourceGroupName, vnetName, vnet);
                await putVnetResponseOperation.WaitForCompletionAsync();
                // Create a Subnet
                // Populate paramters for a Subnet
                Subnet subnet = new Subnet()
                {
                    Name = subnet2Name,
                    AddressPrefix = "10.0.1.0/24",
                };

                #region Verification
                SubnetsCreateOrUpdateOperation putSubnetResponseOperation = await NetworkManagementClient.GetSubnetsClient().StartCreateOrUpdateAsync(resourceGroupName, vnetName, subnet2Name, subnet);
                await putSubnetResponseOperation.WaitForCompletionAsync();
                Response<VirtualNetwork> getVnetResponse = await NetworkManagementClient.GetVirtualNetworksClient().GetAsync(resourceGroupName, vnetName);
                Assert.AreEqual(2, getVnetResponse.Value.Subnets.Count());

                Response<Subnet> getSubnetResponse = await NetworkManagementClient.GetSubnetsClient().GetAsync(resourceGroupName, vnetName, subnet2Name);

                // Verify the getSubnetResponse
                Assert.True(AreSubnetsEqual(getVnetResponse.Value.Subnets[1], getSubnetResponse));

                AsyncPageable<Subnet> getSubnetListResponseAP = NetworkManagementClient.GetSubnetsClient().ListAsync(resourceGroupName, vnetName);
                List<Subnet> getSubnetListResponse = await getSubnetListResponseAP.ToEnumerableAsync();
                // Verify ListSubnets
                Assert.True(AreSubnetsEqual(getVnetResponse.Value.Subnets, getSubnetListResponse));

                // Delete the subnet "subnet1"
                await NetworkManagementClient.GetSubnetsClient().StartDeleteAsync(resourceGroupName, vnetName, subnet2Name);

                // Verify that the deletion was successful
                getSubnetListResponseAP = NetworkManagementClient.GetSubnetsClient().ListAsync(resourceGroupName, vnetName);
                getSubnetListResponse = await getSubnetListResponseAP.ToEnumerableAsync();
                Has.One.EqualTo(getSubnetListResponse);
                Assert.AreEqual(subnet1Name, getSubnetListResponse.ElementAt(0).Name);
                #endregion
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
            }
        }

        [Test]
        [Ignore("Track2: Need  RedisManagementClient")]
        public async Task SubnetResourceNavigationLinksTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("csmrg");

            try
            {
                string location = await NetworkManagementTestUtilities.GetResourceLocation(ResourceManagementClient, "Microsoft.Network/virtualNetworks");
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
                string vnetName = Recording.GenerateAssetName("azsmnet");
                string subnetName = Recording.GenerateAssetName("azsmnet");

                VirtualNetwork vnet = new VirtualNetwork()
                {
                    Location = location,
                    AddressSpace = new AddressSpace()
                    {
                        AddressPrefixes = new List<string>() { "10.0.0.0/16", }
                    },
                    DhcpOptions = new DhcpOptions()
                    {
                        DnsServers = new List<string>() { "10.1.1.1", "10.1.2.4" }
                    },
                    Subnets = new List<Subnet>() { new Subnet() { Name = subnetName, AddressPrefix = "10.0.0.0/24", } }
                };

                VirtualNetworksCreateOrUpdateOperation putVnetResponseOperation = await NetworkManagementClient.GetVirtualNetworksClient().StartCreateOrUpdateAsync(resourceGroupName, vnetName, vnet);
                await putVnetResponseOperation.WaitForCompletionAsync();
                Response<Subnet> getSubnetResponse = await NetworkManagementClient.GetSubnetsClient().GetAsync(resourceGroupName, vnetName, subnetName);
                Assert.Null(getSubnetResponse.Value.ResourceNavigationLinks);

                //TODO:Need RedisManagementClient
                //redisClient.Redis.CreateOrUpdateWithHttpMessagesAsync(resourceGroupName, redisName, parameters: new RedisCreateOrUpdateParameters
                //{
                //    Location = location,
                //    Sku = new Microsoft.Azure.Management.Redis.Models.Sku()
                //    {
                //        Name = SkuName.Premium,
                //        Family = SkuFamily.P,
                //        Capacity = 1
                //    },
                //    SubnetId = getSubnetResponse.Id
                //}).Wait();

                // wait for maximum 30 minutes for cache to create
                //for (int i = 0; i < 60; i++)
                //{
                //    TestUtilities.Wait(new TimeSpan(0, 0, 30));
                //    RedisResource responseGet = redisClient.Redis.Get(resourceGroupName: resourceGroupName, name: redisName);
                //    if ("succeeded".Equals(responseGet.ProvisioningState, StringComparison.OrdinalIgnoreCase))
                //    {
                //        break;
                //    }
                //    Assert.False(i == 60, "Cache is not in succeeded state even after 30 min.");
                //}

                getSubnetResponse = await NetworkManagementClient.GetSubnetsClient().GetAsync(resourceGroupName, vnetName, subnetName);
                Assert.AreEqual(1, getSubnetResponse.Value.ResourceNavigationLinks.Count);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
            }
        }

        private bool AreSubnetsEqual(Subnet subnet1, Subnet subnet2)
        {
            return subnet1.Id == subnet2.Id &&
                   subnet1.Etag == subnet2.Etag &&
                   subnet1.ProvisioningState == subnet2.ProvisioningState &&
                   subnet1.Name == subnet2.Name &&
                   subnet1.AddressPrefix == subnet2.AddressPrefix;
        }

        private bool AreSubnetsEqual(IEnumerable<Subnet> subnets1, IEnumerable<Subnet> subnets2)
        {
            var subnetCollection = subnets1.Zip(subnets2, (s1, s2) => new { subnet1 = s1, subnet2 = s2 });

            return subnetCollection.All(subnets => AreSubnetsEqual(subnets.subnet1, subnets.subnet2));
        }
    }
}
