// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class VerifyIpFlowTests : NetworkTestsManagementClientBase
    {
        public VerifyIpFlowTests(bool isAsync) : base(isAsync)
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
        public async Task VerifyIpFlowApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            try
            {
                string location = "westus2";
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));

                string virtualMachineName1 = Recording.GenerateAssetName("azsmnet");
                string networkInterfaceName1 = Recording.GenerateAssetName("azsmnet");
                string networkSecurityGroupName = virtualMachineName1 + "-nsg";

                //Deploy VM with a template
                await Deployments.CreateVm(
                    resourcesClient: ResourceManagementClient,
                    resourceGroupName: resourceGroupName,
                    location: location,
                    virtualMachineName: virtualMachineName1,
                    storageAccountName: Recording.GenerateAssetName("azsmnet"),
                    networkInterfaceName: networkInterfaceName1,
                    networkSecurityGroupName: networkSecurityGroupName,
                    diagnosticsStorageAccountName: Recording.GenerateAssetName("azsmnet"),
                    deploymentName: Recording.GenerateAssetName("azsmnet")
                    );

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync(resourceGroupName, networkWatcherName, properties);

                Response<Compute.Models.VirtualMachine> getVm1 = await ComputeManagementClient.GetVirtualMachinesClient().GetAsync(resourceGroupName, virtualMachineName1);
                string localIPAddress = NetworkManagementClient.GetNetworkInterfacesClient().GetAsync(resourceGroupName, networkInterfaceName1).Result.Value.IpConfigurations.FirstOrDefault().PrivateIPAddress;

                string securityRule1 = Recording.GenerateAssetName("azsmnet");

                // Add a security rule
                SecurityRule SecurityRule = new SecurityRule()
                {
                    Name = securityRule1,
                    Access = SecurityRuleAccess.Deny,
                    Description = "Test outbound security rule",
                    DestinationAddressPrefix = "*",
                    DestinationPortRange = "80",
                    Direction = SecurityRuleDirection.Outbound,
                    Priority = 501,
                    Protocol = SecurityRuleProtocol.Tcp,
                    SourceAddressPrefix = "*",
                    SourcePortRange = "*",
                };

                Response<NetworkSecurityGroup> nsg = await NetworkManagementClient.GetNetworkSecurityGroupsClient().GetAsync(resourceGroupName, networkSecurityGroupName);
                nsg.Value.SecurityRules.Add(SecurityRule);
                NetworkSecurityGroupsCreateOrUpdateOperation createOrUpdateOperation = await NetworkManagementClient.GetNetworkSecurityGroupsClient().StartCreateOrUpdateAsync(resourceGroupName, networkSecurityGroupName, nsg);
                await createOrUpdateOperation.WaitForCompletionAsync();

                VerificationIPFlowParameters ipFlowProperties = new VerificationIPFlowParameters(getVm1.Value.Id, "Outbound", "TCP", "80", "80", localIPAddress, "12.11.12.14");

                //Verify IP flow from a VM to a location given the configured  rule
                NetworkWatchersVerifyIPFlowOperation verifyIpFlowOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartVerifyIPFlowAsync("NetworkWatcherRG", "NetworkWatcher_westus2", ipFlowProperties);
                Response<VerificationIPFlowResult> verifyIpFlow = await verifyIpFlowOperation.WaitForCompletionAsync();
                //Verify validity of the result
                Assert.AreEqual("Deny", verifyIpFlow.Value.Access.ToString());
                Assert.AreEqual("securityRules/" + securityRule1, verifyIpFlow.Value.RuleName);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }
    }
}
