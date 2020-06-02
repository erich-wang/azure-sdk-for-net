// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
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
    public class ConnectionMonitorTests : NetworkTestsManagementClientBase
    {
        public ConnectionMonitorTests(bool isAsync) : base(isAsync)
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
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task PutConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName = "cm";
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30
                };

                Operation<ConnectionMonitorResult> putConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                Response<ConnectionMonitorResult> putConnectionMonitor = await putConnectionMonitorOperation.WaitForCompletionAsync();

                Assert.AreEqual("Running", putConnectionMonitor.Value.MonitoringStatus);
                Assert.AreEqual("centraluseuap", putConnectionMonitor.Value.Location);
                Assert.AreEqual(30, putConnectionMonitor.Value.MonitoringIntervalInSeconds);
                Assert.AreEqual(connectionMonitorName, putConnectionMonitor.Value.Name);
                Assert.AreEqual(getVm.Value.Id, putConnectionMonitor.Value.Source.ResourceId);
                Assert.AreEqual("bing.com", putConnectionMonitor.Value.Destination.Address);
                Assert.AreEqual(80, putConnectionMonitor.Value.Destination.Port);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }

        [Test]
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task StartConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName = Recording.GenerateAssetName("azsmnet");
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30,
                    AutoStart = false
                };

                Operation<ConnectionMonitorResult> putConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                Response<ConnectionMonitorResult> putConnectionMonitor = await putConnectionMonitorOperation.WaitForCompletionAsync();
                Assert.AreEqual("NotStarted", putConnectionMonitor.Value.MonitoringStatus);

                ConnectionMonitorsStartOperation connectionMonitorsStartOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartStartAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                await connectionMonitorsStartOperation.WaitForCompletionAsync();

                Response<ConnectionMonitorResult> getConnectionMonitor = await NetworkManagementClient.GetConnectionMonitorsClient().GetAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                Assert.AreEqual("Running", getConnectionMonitor.Value.MonitoringStatus);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }

        [Test]
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task StopConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName = Recording.GenerateAssetName("azsmnet");
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30
                };

                Operation<ConnectionMonitorResult> putConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                Response<ConnectionMonitorResult> putConnectionMonitor = await putConnectionMonitorOperation.WaitForCompletionAsync();
                Assert.AreEqual("Running", putConnectionMonitor.Value.MonitoringStatus);

                ConnectionMonitorsStopOperation connectionMonitorsStopOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartStopAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                await connectionMonitorsStopOperation.WaitForCompletionAsync();

                Response<ConnectionMonitorResult> getConnectionMonitor = await NetworkManagementClient.GetConnectionMonitorsClient().GetAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                Assert.AreEqual("Stopped", getConnectionMonitor.Value.MonitoringStatus);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }

        [Test]
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task QueryConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName = Recording.GenerateAssetName("azsmnet");
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30
                };

                Operation<ConnectionMonitorResult> putConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                await putConnectionMonitorOperation.WaitForCompletionAsync();

                ConnectionMonitorsStartOperation connectionMonitorsStartOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartStartAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                await connectionMonitorsStartOperation.WaitForCompletionAsync();

                ConnectionMonitorsStopOperation connectionMonitorsStopOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartStopAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                await connectionMonitorsStopOperation.WaitForCompletionAsync();

                Operation<ConnectionMonitorQueryResult> queryResultOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartQueryAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName);
                Response<ConnectionMonitorQueryResult> queryResult = await queryResultOperation.WaitForCompletionAsync();
                //Has.One.EqualTo(queryResult.States);
                Assert.AreEqual("Reachable", queryResult.Value.States[0].ConnectionState);
                Assert.AreEqual("InProgress", queryResult.Value.States[0].EvaluationState);
                Assert.AreEqual(2, queryResult.Value.States[0].Hops.Count);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }

        [Test]
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task UpdateConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName = Recording.GenerateAssetName("azsmnet");
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30
                };

                Operation<ConnectionMonitorResult> putConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                Response<ConnectionMonitorResult> putConnectionMonitor = await putConnectionMonitorOperation.WaitForCompletionAsync();
                Assert.AreEqual(30, putConnectionMonitor.Value.MonitoringIntervalInSeconds);

                cm.MonitoringIntervalInSeconds = 60;
                Operation<ConnectionMonitorResult> updateConnectionMonitorOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName, cm);
                Response<ConnectionMonitorResult> updateConnectionMonitor = await updateConnectionMonitorOperation.WaitForCompletionAsync();
                Assert.AreEqual(60, updateConnectionMonitor.Value.MonitoringIntervalInSeconds);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }

        [Test]
        [Ignore("ApiVersion does not meet the requirements")]
        public async Task DeleteConnectionMonitorTest()
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
                await createOrUpdateOperation.WaitForCompletionAsync();

                //TODO:There is no need to perform a separate create NetworkWatchers operation
                //Create network Watcher
                //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                //NetworkWatcher properties = new NetworkWatcher { Location = location };
                //await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", properties);

                string connectionMonitorName1 = Recording.GenerateAssetName("azsmnet");
                string connectionMonitorName2 = Recording.GenerateAssetName("azsmnet");
                ConnectionMonitor cm = new ConnectionMonitor
                {
                    Location = location,
                    Source = new ConnectionMonitorSource(getVm.Value.Id),
                    Destination = new ConnectionMonitorDestination
                    {
                        Address = "bing.com",
                        Port = 80
                    },
                    MonitoringIntervalInSeconds = 30,
                    AutoStart = false
                };

                Operation<ConnectionMonitorResult> connectionMonitor1Operation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName1, cm);
                await connectionMonitor1Operation.WaitForCompletionAsync();
                Operation<ConnectionMonitorResult> connectionMonitor2Operation = await NetworkManagementClient.GetConnectionMonitorsClient().StartCreateOrUpdateAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName2, cm);
                await connectionMonitor2Operation.WaitForCompletionAsync();

                AsyncPageable<ConnectionMonitorResult> getConnectionMonitors1AP = NetworkManagementClient.GetConnectionMonitorsClient().ListAsync("NetworkWatcherRG", "NetworkWatcher_westus2");
                Task<List<ConnectionMonitorResult>> getConnectionMonitors1 = getConnectionMonitors1AP.ToEnumerableAsync();
                Assert.AreEqual(2, getConnectionMonitors1.Result.Count);

                ConnectionMonitorsDeleteOperation connectionMonitorsDeleteOperation = await NetworkManagementClient.GetConnectionMonitorsClient().StartDeleteAsync("NetworkWatcherRG", "NetworkWatcher_westus2", connectionMonitorName2);
                await connectionMonitorsDeleteOperation.WaitForCompletionAsync();
                AsyncPageable<ConnectionMonitorResult> getConnectionMonitors2 = NetworkManagementClient.GetConnectionMonitorsClient().ListAsync("NetworkWatcherRG", "NetworkWatcher_westus2");
                Has.One.EqualTo(getConnectionMonitors2);
            }
            finally
            {
                ResourceGroupsDeleteOperation deleteOperation = await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
                await deleteOperation.WaitForCompletionAsync();
            }
        }
    }
}
