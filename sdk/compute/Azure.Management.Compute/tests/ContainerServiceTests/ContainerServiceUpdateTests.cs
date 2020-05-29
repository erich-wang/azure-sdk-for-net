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
using Azure.Core.TestFramework;
using System.Reflection;
using System.Threading;
using Azure.Management.Compute.Tests;
using Azure.Management.Compute;
using Azure.Management.Compute.Models;
using System.Text.Json;
using System.Net;

namespace Azure.Management.Compute.Tests
{
    public class ContainerServiceUpdateTests : ContainerServiceTestsBase
    {
        public ContainerServiceUpdateTests(bool isAsync)
           : base(isAsync)
        {
        }
        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                InitializeBase();
            }
            //ComputeManagementClient computeClient;
            //ResourceManagementClient resourcesClient;
        }
        [Test]
        public async Task TestContainerServiceUpdateOperations()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var csName = Recording.GenerateAssetName(ContainerServiceNamePrefix);
            var masterDnsPrefixName = Recording.GenerateAssetName(MasterProfileDnsPrefix);
            var agentPoolDnsPrefixName = Recording.GenerateAssetName(AgentPoolProfileDnsPrefix);
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "australiasoutheast");
                EnsureClientsInitialized();

                ContainerService inputContainerService;
                var getTwocontainerService = await CreateContainerService_NoAsyncTracking(
                    rgName,
                    csName,
                    masterDnsPrefixName,
                    agentPoolDnsPrefixName, cs =>
                    {
                        cs.AgentPoolProfiles[0].Count = 1;
                        cs.MasterProfile.Count = 1;
                    });
                var containerService = getTwocontainerService.Item1;
                inputContainerService = getTwocontainerService.Item2;
                // Update Container Service with increased AgentPoolProfiles Count
                containerService.AgentPoolProfiles[0].Count = 2;
                UpdateContainerService(rgName, csName, containerService);

                containerService = await ContainerServicesClient.GetAsync(rgName, containerService.Name);
                ValidateContainerService(containerService, containerService);

                var listRes = ContainerServicesClient.ListByResourceGroupAsync(rgName);
                var listResult = await listRes.ToEnumerableAsync();
                //Assert.Contains(listResult, a => a.Name == containerService.Name);
                await ContainerServicesClient.StartDeleteAsync(rgName, containerService.Name);
                var listResultAfterDeletionResult = ContainerServicesClient.ListByResourceGroupAsync(rgName);
                var listResultAfterDeletion = await listResultAfterDeletionResult.ToEnumerableAsync();
                Assert.True(!listResultAfterDeletion.Any());
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }
    }
}
