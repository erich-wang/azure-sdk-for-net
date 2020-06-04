﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Azure.Management.Compute.Tests
{
    public class ContainerServiceOperationalTests : ContainerServiceTestsBase
    {
        public ContainerServiceOperationalTests(bool isAsync)
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
        }

        [Test]
        [Ignore ("This case should be tested by compute team because of ex 'Address prefix string for resource /subscriptions/c9cbd920-c00c-427c-852b-8aaf38badaeb/resourceGroups/crptestar12191/providers/Microsoft.Network/virtualNetworks/dcos-vnet-35162761 cannot be null or empty' when record in track2")]
        public async Task TestDCOSOperations()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
                // Create resource group
                var rgName = Recording.GenerateAssetName(TestPrefix) + 1;
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
                        agentPoolDnsPrefixName,
                        //out inputContainerService,
                        cs => cs.OrchestratorProfile.OrchestratorType = ContainerServiceOrchestratorTypes.Dcos);
                var containerService = getTwocontainerService.Item1;
                inputContainerService = getTwocontainerService.Item2;
                    await WaitForCompletionAsync(await ContainerServicesClient.StartDeleteAsync(rgName, containerService.Name));
                }
                finally
                {
                    Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                    // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                    // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                    await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
                }
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Container Service
        /// Get Container Service
        /// Delete Container Service
        /// Delete RG
        /// </summary>
        [Test]
        [Ignore("this should be tested by generate team")]
        public async Task TestSwarmOperations()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix) + 1;
            var csName = Recording.GenerateAssetName(ContainerServiceNamePrefix);
            var masterDnsPrefixName = Recording.GenerateAssetName(MasterProfileDnsPrefix);
            var agentPoolDnsPrefixName = Recording.GenerateAssetName(AgentPoolProfileDnsPrefix);
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "australiasoutheast");
                EnsureClientsInitialized();
                ContainerService inputContainerService;
                var getTwocontainerService =await CreateContainerService_NoAsyncTracking(
                    rgName,
                    csName,
                    masterDnsPrefixName,
                    agentPoolDnsPrefixName,
                    //out inputContainerService,
                    cs => cs.OrchestratorProfile.OrchestratorType = ContainerServiceOrchestratorTypes.Swarm);
                var containerService = getTwocontainerService.Item1;
                inputContainerService = getTwocontainerService.Item2;
                await WaitForCompletionAsync (await ContainerServicesClient.StartDeleteAsync(rgName, containerService.Name));
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
    }
}
