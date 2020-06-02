// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute;
using Azure.Management.Compute.Models;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class PacketCaptureTests : NetworkTestsManagementClientBase
    {
        public PacketCaptureTests(bool isAsync) : base(isAsync)
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
        public async Task PacketCaptureApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            try
            {
                string location = "westus2";
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));
                string virtualMachineName = Recording.GenerateAssetName("azsmnet");
                string networkInterfaceName = Recording.GenerateAssetName("azsmnet");
                string networkSecurityGroupName = virtualMachineName + "-nsg";

                //Deploy VM with template
                await Deployments.CreateVm(
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

                VirtualMachineExtensionsCreateOrUpdateOperation createOrUpdateOperation = await ComputeManagementClient.GetVirtualMachineExtensionsClient().StartCreateOrUpdateAsync(resourceGroupName, getVm.Value.Name, "NetworkWatcherAgent", parameters);
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string pcName1 = "pc1";
                string pcName2 = "pc2";

                PacketCapture pcProperties = new PacketCapture(getVm.Value.Id, new PacketCaptureStorageLocation { FilePath = @"C:\tmp\Capture.cap" });

                PacketCapturesCreateOperation createPacketCapture1Operation = await NetworkManagementClient.GetPacketCapturesClient().StartCreateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1, pcProperties);
                Response<PacketCaptureResult> createPacketCapture1 = await createPacketCapture1Operation.WaitForCompletionAsync();
                Response<PacketCaptureResult> getPacketCapture = await NetworkManagementClient.GetPacketCapturesClient().GetAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1);
                PacketCapturesGetStatusOperation queryPCOperation = await NetworkManagementClient.GetPacketCapturesClient().StartGetStatusAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1);
                await queryPCOperation.WaitForCompletionAsync();

                //Validation
                Assert.AreEqual(pcName1, createPacketCapture1.Value.Name);
                Assert.AreEqual(1073741824, createPacketCapture1.Value.TotalBytesPerSession);
                Assert.AreEqual(0, createPacketCapture1.Value.BytesToCapturePerPacket);
                Assert.AreEqual(18000, createPacketCapture1.Value.TimeLimitInSeconds);
                Assert.AreEqual(@"C:\tmp\Capture.cap", createPacketCapture1.Value.StorageLocation.FilePath);
                Assert.AreEqual("Succeeded", getPacketCapture.Value.ProvisioningState.ToString());

                PacketCapturesCreateOperation packetCapturesCreateOperation = await NetworkManagementClient.GetPacketCapturesClient().StartCreateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName2, pcProperties);
                await packetCapturesCreateOperation.WaitForCompletionAsync();

                AsyncPageable<PacketCaptureResult> listPCByRg1AP = NetworkManagementClient.GetPacketCapturesClient().ListAsync("NetworkWatcherRG", "NetworkWatcher_westus2");
                List<PacketCaptureResult> listPCByRg1 = await listPCByRg1AP.ToEnumerableAsync();

                PacketCapturesStopOperation packetCapturesStopOperation = await NetworkManagementClient.GetPacketCapturesClient().StartStopAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1);
                await packetCapturesStopOperation.WaitForCompletionAsync();

                PacketCapturesGetStatusOperation queryPCAfterStopOperation = await NetworkManagementClient.GetPacketCapturesClient().StartGetStatusAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1);
                Response<PacketCaptureQueryStatusResult> queryPCAfterStop = await queryPCAfterStopOperation.WaitForCompletionAsync();

                PacketCapturesDeleteOperation packetCapturesDeleteOperation = await NetworkManagementClient.GetPacketCapturesClient().StartDeleteAsync("NetworkWatcherRG", "NetworkWatcher_westus2", pcName1);
                await packetCapturesDeleteOperation.WaitForCompletionAsync();
                AsyncPageable<PacketCaptureResult> listPCByRg2 = NetworkManagementClient.GetPacketCapturesClient().ListAsync("NetworkWatcherRG", "NetworkWatcher_westus2");

                //Validation
                Assert.AreEqual(2, listPCByRg1.Count());
                Assert.AreEqual("Stopped", queryPCAfterStop.Value.PacketCaptureStatus.ToString());
                Assert.AreEqual("Manual", queryPCAfterStop.Value.StopReason);
                Has.One.EqualTo(listPCByRg2);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }
    }
}
