// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using NUnit.Framework;

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
        }

        [Test]
        [Ignore("need to be tested by compute team because of the ex' cannot unmarshal string into Go struct field Properties.properties.masterProfile of type int.'")]
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
                //Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "australiasoutheast");
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
                await WaitForCompletionAsync( await ContainerServicesClient.StartDeleteAsync(rgName, containerService.Name));
                var listResultAfterDeletionResult = ContainerServicesClient.ListByResourceGroupAsync(rgName);
                var listResultAfterDeletion = await listResultAfterDeletionResult.ToEnumerableAsync();
                Assert.True(!listResultAfterDeletion.Any());
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
    }
}
