// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    public class FlowLogTests : NetworkTestsManagementClientBase
    {
        public FlowLogTests(bool isAsync) : base(isAsync)
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
        [Ignore("Need OperationalInsightsManagementClient")]
        public async Task FlowLogApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            try
            {
                string location = "eastus2euap";
                //string workspaceLocation = "East US";
                await ResourceGroupsClient.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(location));

                //Create network security group
                string networkSecurityGroupName = Recording.GenerateAssetName("azsmnet");
                var networkSecurityGroup = new NetworkSecurityGroup() { Location = location, };

                // Put Nsg
                NetworkSecurityGroupsCreateOrUpdateOperation putNsgResponseOperation = await NetworkManagementClient.GetNetworkSecurityGroupsClient().StartCreateOrUpdateAsync(resourceGroupName, networkSecurityGroupName, networkSecurityGroup);
                await putNsgResponseOperation.WaitForCompletionAsync();
                // Get NSG
                Response<NetworkSecurityGroup> getNsgResponse = await NetworkManagementClient.GetNetworkSecurityGroupsClient().GetAsync(resourceGroupName, networkSecurityGroupName);

                string networkWatcherName = Recording.GenerateAssetName("azsmnet");
                NetworkWatcher properties = new NetworkWatcher { Location = location };

                //Create network Watcher
                await NetworkManagementClient.GetNetworkWatchersClient().CreateOrUpdateAsync(resourceGroupName, networkWatcherName, properties);

                //Create storage
                string storageName = Recording.GenerateAssetName("azsmnet");

                var storageParameters = new StorageAccountCreateParameters(new Storage.Models.Sku(SkuName.StandardLRS), Kind.Storage, location);

                Operation<StorageAccount> storageAccountOperation = await StorageManagementClient.GetStorageAccountsClient().StartCreateAsync(resourceGroupName, storageName, storageParameters);
                Response<StorageAccount> storageAccount = await storageAccountOperation.WaitForCompletionAsync();

                //create workspace
                string workspaceName = Recording.GenerateAssetName("azsmnet");

                //TODO:Need OperationalInsightsManagementClient SDK
                //var workSpaceParameters = new Workspace()
                //{
                //    Location = workspaceLocation
                //};
                //var workspace = operationalInsightsManagementClient.Workspaces.CreateOrUpdate(resourceGroupName, workspaceName, workSpaceParameters);

                FlowLogInformation configParameters = new FlowLogInformation(getNsgResponse.Value.Id, storageAccount.Value.Id, true)
                {
                    RetentionPolicy = new RetentionPolicyParameters
                    {
                        Days = 5,
                        Enabled = true
                    },
                    FlowAnalyticsConfiguration = new TrafficAnalyticsProperties()
                    {
                        NetworkWatcherFlowAnalyticsConfiguration = new TrafficAnalyticsConfigurationProperties()
                        {
                            Enabled = true,
                            //WorkspaceId = workspace.CustomerId,
                            //WorkspaceRegion = workspace.Location,
                            //WorkspaceResourceId = workspace.Id
                        }
                    }
                };

                //configure flowlog and TA
                NetworkWatchersSetFlowLogConfigurationOperation configureFlowLog1Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartSetFlowLogConfigurationAsync(resourceGroupName, networkWatcherName, configParameters);
                await configureFlowLog1Operation.WaitForCompletionAsync();
                FlowLogStatusParameters flowLogParameters = new FlowLogStatusParameters(getNsgResponse.Value.Id);

                NetworkWatchersGetFlowLogStatusOperation queryFlowLogStatus1Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetFlowLogStatusAsync(resourceGroupName, networkWatcherName, flowLogParameters);
                Response<FlowLogInformation> queryFlowLogStatus1 = await queryFlowLogStatus1Operation.WaitForCompletionAsync();
                //check both flowlog and TA config and enabled status
                Assert.AreEqual(queryFlowLogStatus1.Value.TargetResourceId, configParameters.TargetResourceId);
                Assert.True(queryFlowLogStatus1.Value.Enabled);
                Assert.AreEqual(queryFlowLogStatus1.Value.StorageId, configParameters.StorageId);
                Assert.AreEqual(queryFlowLogStatus1.Value.RetentionPolicy.Days, configParameters.RetentionPolicy.Days);
                Assert.AreEqual(queryFlowLogStatus1.Value.RetentionPolicy.Enabled, configParameters.RetentionPolicy.Enabled);
                Assert.True(queryFlowLogStatus1.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.Enabled);
                Assert.AreEqual(queryFlowLogStatus1.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceId,
                    configParameters.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceId);
                Assert.AreEqual(queryFlowLogStatus1.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceRegion,
                    configParameters.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceRegion);
                Assert.AreEqual(queryFlowLogStatus1.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceResourceId,
                    configParameters.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.WorkspaceResourceId);

                //disable TA
                configParameters.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.Enabled = false;
                NetworkWatchersSetFlowLogConfigurationOperation configureFlowLog2Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartSetFlowLogConfigurationAsync(resourceGroupName, networkWatcherName, configParameters);
                await configureFlowLog2Operation.WaitForCompletionAsync();
                NetworkWatchersGetFlowLogStatusOperation queryFlowLogStatus2Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetFlowLogStatusAsync(resourceGroupName, networkWatcherName, flowLogParameters);
                Response<FlowLogInformation> queryFlowLogStatus2 = await queryFlowLogStatus2Operation.WaitForCompletionAsync();

                //check TA disabled and ensure flowlog config is unchanged
                Assert.AreEqual(queryFlowLogStatus2.Value.TargetResourceId, configParameters.TargetResourceId);
                Assert.True(queryFlowLogStatus2.Value.Enabled);
                Assert.AreEqual(queryFlowLogStatus2.Value.StorageId, configParameters.StorageId);
                Assert.AreEqual(queryFlowLogStatus2.Value.RetentionPolicy.Days, configParameters.RetentionPolicy.Days);
                Assert.AreEqual(queryFlowLogStatus2.Value.RetentionPolicy.Enabled, configParameters.RetentionPolicy.Enabled);
                Assert.False(queryFlowLogStatus2.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.Enabled);

                //disable flowlog (and TA)
                configParameters.Enabled = false;
                NetworkWatchersSetFlowLogConfigurationOperation configureFlowLog3Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartSetFlowLogConfigurationAsync(resourceGroupName, networkWatcherName, configParameters);
                await configureFlowLog3Operation.WaitForCompletionAsync();
                NetworkWatchersGetFlowLogStatusOperation queryFlowLogStatus3Operation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetFlowLogStatusAsync(resourceGroupName, networkWatcherName, flowLogParameters);
                Response<FlowLogInformation> queryFlowLogStatus3 = await queryFlowLogStatus3Operation.WaitForCompletionAsync();

                //check both flowlog and TA disabled
                Assert.False(queryFlowLogStatus3.Value.Enabled);
                Assert.False(queryFlowLogStatus3.Value.FlowAnalyticsConfiguration.NetworkWatcherFlowAnalyticsConfiguration.Enabled);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
            }
        }
    }
}
