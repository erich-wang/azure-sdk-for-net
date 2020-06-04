﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class CheckConnectivityTests : NetworkTestsManagementClientBase
    {
        public CheckConnectivityTests(bool isAsync) : base(isAsync)
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
        [Category("Must be online")]
        public async Task CheckConnectivityVmToInternetTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            try
            {
                string location = "westus2";
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
                string virtualMachineName = Recording.GenerateAssetName("azsmnet");
                string networkInterfaceName = Recording.GenerateAssetName("azsmnet");
                string networkSecurityGroupName = virtualMachineName + "-nsg";

                //Deploy VM with a template
                await DeploymentUpdate.CreateVm(
                    resourcesClient: ResourceManagementClient,
                    resourceGroupName: resourceGroupName,
                    location: location,
                    virtualMachineName: virtualMachineName,
                    storageAccountName: Recording.GenerateAssetName("azsmnet"),
                    networkInterfaceName: networkInterfaceName,
                    networkSecurityGroupName: networkSecurityGroupName,
                    diagnosticsStorageAccountName: Recording.GenerateAssetName("azsmnet"),
                    deploymentName: Recording.GenerateAssetName("azsmnet")
                    );

                Response<VirtualMachine> getVm = await ComputeManagementClient.GetVirtualMachinesClient().GetAsync(resourceGroupName, virtualMachineName);

                //Deploy networkWatcherAgent on VM
                VirtualMachineExtension parameters = new VirtualMachineExtension(location)
                {
                    Publisher = "Microsoft.Azure.NetworkWatcher",
                    TypeHandlerVersion = "1.4",
                    TypePropertiesType = "NetworkWatcherAgentWindows"
                };

                Compute.VirtualMachineExtensionsCreateOrUpdateOperation createOrUpdateOperation = await ComputeManagementClient.GetVirtualMachineExtensionsClient().StartCreateOrUpdateAsync(resourceGroupName, getVm.Value.Name, "NetworkWatcherAgent", parameters);
                await WaitForCompletionAsync(createOrUpdateOperation);

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                ConnectivityParameters connectivityParameters =
                    new ConnectivityParameters(new ConnectivitySource(getVm.Value.Id), new ConnectivityDestination { Address = "bing.com", Port = 80 });

                Operation<ConnectivityInformation> connectivityCheckOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartCheckConnectivityAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectivityParameters);
                Response<ConnectivityInformation> connectivityCheck = await WaitForCompletionAsync(connectivityCheckOperation);

                //Validation
                Assert.AreEqual("Reachable", connectivityCheck.Value.ConnectionStatus.ToString());
                Assert.AreEqual(0, connectivityCheck.Value.ProbesFailed);
                Assert.AreEqual("Source", connectivityCheck.Value.Hops.FirstOrDefault().Type);
                Assert.AreEqual("Internet", connectivityCheck.Value.Hops.LastOrDefault().Type);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await WaitForCompletionAsync(deleteOperation);
            }
        }
    }
}
