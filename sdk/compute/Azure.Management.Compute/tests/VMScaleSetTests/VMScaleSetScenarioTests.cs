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
     public class VMScaleSetScenarioTests : VMScaleSetVMTestsBase
    {
        public VMScaleSetScenarioTests(bool isAsync)
        : base(isAsync)
        {
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet with extension
        /// Get VMScaleSet Model View
        /// Get VMScaleSet Instance View
        /// List VMScaleSets in a RG
        /// List Available Skus
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations")]
        public async Task TestVMScaleSetScenarioOperations()
        {
            await TestScaleSetOperationsInternal();
        }

        /// <summary>
        /// Covers following Operations for ManagedDisks:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VMScaleSet with extension
        /// Get VMScaleSet Model View
        /// Get VMScaleSet Instance View
        /// List VMScaleSets in a RG
        /// List Available Skus
        /// Delete VMScaleSet
        /// Delete RG
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_ManagedDisks")]
        public async Task TestVMScaleSetScenarioOperations_ManagedDisks_PirImage()
        {
            await TestScaleSetOperationsInternal(hasManagedDisks: true, useVmssExtension: false);
        }

        /// <summary>
        /// To record this test case, you need to run it again zone supported regions like eastus2euap.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_ManagedDisks_PirImage_SingleZone")]
        public async Task TestVMScaleSetScenarioOperations_ManagedDisks_PirImage_SingleZone()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "centralus");
                await TestScaleSetOperationsInternal(hasManagedDisks: true, useVmssExtension: false, zones: new List<string> { "1" });
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it in region which support local diff disks.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_DiffDisks")]
        public async Task TestVMScaleSetScenarioOperations_DiffDisks()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "northeurope");
                await TestScaleSetOperationsInternal(vmSize: VirtualMachineSizeTypes.StandardDS5V2.ToString(), hasManagedDisks: true,
                    hasDiffDisks: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it in region which support DiskEncryptionSet resource for the Disks
        /// </summary>
        [Test]
        [Ignore("this should be tested by generate team")]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_With_DiskEncryptionSet")]
        public async Task TestVMScaleSetScenarioOperations_With_DiskEncryptionSet()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                string diskEncryptionSetId = getDefaultDiskEncryptionSetId();

                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "centraluseuap");
                await TestScaleSetOperationsInternal(vmSize: VirtualMachineSizeTypes.StandardA1V2.ToString(), hasManagedDisks: true, osDiskSizeInGB: 175, diskEncryptionSetId: diskEncryptionSetId);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_UltraSSD")]
        public async Task TestVMScaleSetScenarioOperations_UltraSSD()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestScaleSetOperationsInternal(vmSize: VirtualMachineSizeTypes.StandardE4SV3.ToString(), hasManagedDisks: true,
                        useVmssExtension: false, zones: new List<string> { "1" }, enableUltraSSD: true, osDiskSizeInGB: 175);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it again zone supported regions like eastus2euap.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_ManagedDisks_PirImage_Zones")]
        public async Task TestVMScaleSetScenarioOperations_ManagedDisks_PirImage_Zones()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "centralus");
                await TestScaleSetOperationsInternal(
                    hasManagedDisks: true,
                    useVmssExtension: false,
                    zones: new List<string> { "1", "3" },
                    osDiskSizeInGB: 175);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it again zone supported regions like eastus2euap.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_PpgScenario")]
        public async Task TestVMScaleSetScenarioOperations_PpgScenario()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestScaleSetOperationsInternal(hasManagedDisks: true, useVmssExtension: false, isPpgScenario: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_ScheduledEvents")]
        public async Task TestVMScaleSetScenarioOperations_ScheduledEvents()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestScaleSetOperationsInternal(hasManagedDisks: true, useVmssExtension: false,
                    vmScaleSetCustomizer:
                    vmScaleSet =>
                    {
                        vmScaleSet.VirtualMachineProfile.ScheduledEventsProfile = new ScheduledEventsProfile
                        {
                            TerminateNotificationProfile = new TerminateNotificationProfile
                            {
                                Enable = true,
                                NotBeforeTimeout = "PT6M",
                            }
                        };
                    },
                    vmScaleSetValidator: vmScaleSet =>
                    {
                        Assert.True(true == vmScaleSet.VirtualMachineProfile.ScheduledEventsProfile?.TerminateNotificationProfile?.Enable);
                        Assert.True("PT6M" == vmScaleSet.VirtualMachineProfile.ScheduledEventsProfile?.TerminateNotificationProfile?.NotBeforeTimeout);
                    });
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_AutomaticRepairsPolicyTest")]
        public async Task TestVMScaleSetScenarioOperations_AutomaticRepairsPolicyTest()
        {
            string environmentVariable = "AZURE_VM_TEST_LOCATION";
            //change the location 'centraluseuap' to 'eastus2'
            string region = "eastus2";
            string originalTestLocation = Environment.GetEnvironmentVariable(environmentVariable);

            try
            {
                Environment.SetEnvironmentVariable(environmentVariable, region);
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

                // Create resource group
                var rgName = Recording.GenerateAssetName(TestPrefix);
                var vmssName = Recording.GenerateAssetName("vmss");
                string storageAccountName = Recording.GenerateAssetName(TestPrefix);
                VirtualMachineScaleSet inputVMScaleSet;

                try
                {
                    var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                    await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");

                    var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                        rgName,
                        vmssName,
                        storageAccountOutput,
                        imageRef,
                        null,
                        (vmScaleSet) =>
                        {
                            vmScaleSet.Overprovision = false;
                        },
                        createWithManagedDisks: true,
                        createWithPublicIpAddress: false,
                        createWithHealthProbe: true);
                    VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                    inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                    // Set Automatic Repairs to true
                    inputVMScaleSet.AutomaticRepairsPolicy = new AutomaticRepairsPolicy()
                    {
                        Enabled = true
                    };
                    UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                    getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                    Assert.NotNull(getResponse.AutomaticRepairsPolicy);
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                    // Update Automatic Repairs default values
                    inputVMScaleSet.AutomaticRepairsPolicy = new AutomaticRepairsPolicy()
                    {
                        Enabled = true,

                        GracePeriod = "PT35M"
                    };
                    UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                    getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                    Assert.NotNull(getResponse.AutomaticRepairsPolicy);
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);

                    // Set automatic repairs to null
                    inputVMScaleSet.AutomaticRepairsPolicy = null;
                    UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                    getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
                    Assert.NotNull(getResponse.AutomaticRepairsPolicy);
                    Assert.True(getResponse.AutomaticRepairsPolicy.Enabled == true);

                    Assert.AreEqual("PT35M", getResponse.AutomaticRepairsPolicy.GracePeriod);

                    // Disable Automatic Repairs
                    inputVMScaleSet.AutomaticRepairsPolicy = new AutomaticRepairsPolicy()
                    {
                        Enabled = false
                    };
                    UpdateVMScaleSet(rgName, vmssName, inputVMScaleSet);

                    getResponse = await VirtualMachineScaleSetsClient.GetAsync(rgName, vmssName);
                    Assert.NotNull(getResponse.AutomaticRepairsPolicy);
                    Assert.True(getResponse.AutomaticRepairsPolicy.Enabled == false);
                }
                finally
                {
                    //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                    //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                    await ResourceGroupsClient.StartDeleteAsync(rgName);
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        [Test]
        //[Trait("Name", "TestVMScaleSetScenarioOperations_OrchestrationService")]
        public async Task TestVMScaleSetScenarioOperations_OrchestrationService()
        {
            string environmentVariable = "AZURE_VM_TEST_LOCATION";
            string region = "northeurope";
            string originalTestLocation = Environment.GetEnvironmentVariable(environmentVariable);

            try
            {
                Environment.SetEnvironmentVariable(environmentVariable, region);
                EnsureClientsInitialized();

                ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

                // Create resource group
                var rgName = Recording.GenerateAssetName(TestPrefix);
                var vmssName = Recording.GenerateAssetName("vmss");
                string storageAccountName = Recording.GenerateAssetName(TestPrefix);
                VirtualMachineScaleSet inputVMScaleSet;

                try
                {
                    var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                    await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");

                    AutomaticRepairsPolicy automaticRepairsPolicy = new AutomaticRepairsPolicy()
                    {
                        Enabled = true
                    };
                    var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                        rgName,
                        vmssName,
                        storageAccountOutput,
                        imageRef,
                        null,
                        (vmScaleSet) =>
                        {
                            vmScaleSet.Overprovision = false;
                        },
                        createWithManagedDisks: true,
                        createWithPublicIpAddress: false,
                        createWithHealthProbe: true,
                        automaticRepairsPolicy: automaticRepairsPolicy);
                    VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                    inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                    ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks: true);
                    var getInstanceViewResponse = (await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName)).Value;

                    Assert.True(getInstanceViewResponse.OrchestrationServices.Count == 1);
                    Assert.AreEqual("Running", getInstanceViewResponse.OrchestrationServices[0].ServiceState);
                    Assert.AreEqual("AutomaticRepairs", getInstanceViewResponse.OrchestrationServices[0].ServiceName);


                    ////TODO
                    OrchestrationServiceStateInput orchestrationServiceStateInput = new OrchestrationServiceStateInput(new OrchestrationServiceSummary().ServiceName,OrchestrationServiceStateAction.Suspend);
                    //OrchestrationServiceStateAction orchestrationServiceStateAction = new OrchestrationServiceStateAction();
                    await VirtualMachineScaleSetsClient.StartSetOrchestrationServiceStateAsync(rgName, vmssName, orchestrationServiceStateInput);

                    getInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                    Assert.AreEqual(OrchestrationServiceState.Suspended.ToString(), getInstanceViewResponse.OrchestrationServices[0].ServiceState);

                    orchestrationServiceStateInput = new OrchestrationServiceStateInput(new OrchestrationServiceSummary().ServiceName, OrchestrationServiceStateAction.Resume);
                    await VirtualMachineScaleSetsClient.StartSetOrchestrationServiceStateAsync(rgName, vmssName, orchestrationServiceStateInput);
                    getInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                    Assert.AreEqual(OrchestrationServiceState.Running.ToString(), getInstanceViewResponse.OrchestrationServices[0].ServiceState);

                    await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmssName);
                }
                finally
                {
                    //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                    //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                    await ResourceGroupsClient.StartDeleteAsync(rgName);
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }


        private async Task TestScaleSetOperationsInternal(string vmSize = null, bool hasManagedDisks = false, bool useVmssExtension = true,
            bool hasDiffDisks = false, IList<string> zones = null, int? osDiskSizeInGB = null, bool isPpgScenario = false, bool? enableUltraSSD = false,
            Action<VirtualMachineScaleSet> vmScaleSetCustomizer = null, Action<VirtualMachineScaleSet> vmScaleSetValidator = null, string diskEncryptionSetId = null)
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            var vmssName = Recording.GenerateAssetName("vmss");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachineScaleSet inputVMScaleSet;

            VirtualMachineScaleSetExtensionProfile extensionProfile = new VirtualMachineScaleSetExtensionProfile()
            {
                Extensions = new List<VirtualMachineScaleSetExtension>()
                {
                    GetTestVMSSVMExtension(),
                }
            };

            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);
                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, "VMScaleSetDoesNotExist");
                string ppgId = null;
                string ppgName = null;
                if (isPpgScenario)
                {
                    ppgName = Recording.GenerateAssetName("ppgtest");
                    ppgId = await CreateProximityPlacementGroup(rgName, ppgName);
                }

                var getTwoVirtualMachineScaleSet = await CreateVMScaleSet_NoAsyncTracking(
                    rgName,
                    vmssName,
                    storageAccountOutput,
                    imageRef,
                    useVmssExtension ? extensionProfile : null,
                    (vmScaleSet) => {
                        vmScaleSet.Overprovision = true;
                        if (!String.IsNullOrEmpty(vmSize))
                        {
                            vmScaleSet.Sku.Name = vmSize;
                        }
                        vmScaleSetCustomizer?.Invoke(vmScaleSet);
                    },
                    createWithManagedDisks: hasManagedDisks,
                    hasDiffDisks: hasDiffDisks,
                    zones: zones,
                    osDiskSizeInGB: osDiskSizeInGB,
                    ppgId: ppgId,
                    enableUltraSSD: enableUltraSSD,
                    diskEncryptionSetId: diskEncryptionSetId);
                VirtualMachineScaleSet getResponse = getTwoVirtualMachineScaleSet.Item1;
                inputVMScaleSet = getTwoVirtualMachineScaleSet.Item2;
                if (diskEncryptionSetId != null)
                {
                    Assert.True(getResponse.VirtualMachineProfile.StorageProfile.OsDisk.ManagedDisk.DiskEncryptionSet != null, "OsDisk.ManagedDisk.DiskEncryptionSet is null");
                    Assert.True(string.Equals(diskEncryptionSetId, getResponse.VirtualMachineProfile.StorageProfile.OsDisk.ManagedDisk.DiskEncryptionSet.Id, StringComparison.OrdinalIgnoreCase),
                        "OsDisk.ManagedDisk.DiskEncryptionSet.Id is not matching with expected DiskEncryptionSet resource");

                    Assert.AreEqual(1, getResponse.VirtualMachineProfile.StorageProfile.DataDisks.Count);
                    Assert.True(getResponse.VirtualMachineProfile.StorageProfile.DataDisks[0].ManagedDisk.DiskEncryptionSet != null, ".DataDisks.ManagedDisk.DiskEncryptionSet is null");
                    Assert.True(string.Equals(diskEncryptionSetId, getResponse.VirtualMachineProfile.StorageProfile.DataDisks[0].ManagedDisk.DiskEncryptionSet.Id, StringComparison.OrdinalIgnoreCase),
                        "DataDisks.ManagedDisk.DiskEncryptionSet.Id is not matching with expected DiskEncryptionSet resource");
                }

                ValidateVMScaleSet(inputVMScaleSet, getResponse, hasManagedDisks, ppgId: ppgId);

                var getInstanceViewResponse = await VirtualMachineScaleSetsClient.GetInstanceViewAsync(rgName, vmssName);
                Assert.NotNull(getInstanceViewResponse);
                ValidateVMScaleSetInstanceView(inputVMScaleSet, getInstanceViewResponse);

                if (isPpgScenario)
                {
                    ProximityPlacementGroup outProximityPlacementGroup = await ProximityPlacementGroupsClient.GetAsync(rgName, ppgName);
                    Assert.AreEqual(1, outProximityPlacementGroup.VirtualMachineScaleSets.Count);
                    string expectedVmssReferenceId = Helpers.GetVMScaleSetReferenceId(m_subId, rgName, vmssName);
                    Assert.AreEqual(expectedVmssReferenceId, outProximityPlacementGroup.VirtualMachineScaleSets.First().Id);
                }

                var listResponse = await (VirtualMachineScaleSetsClient.ListAsync(rgName)).ToEnumerableAsync();
                ValidateVMScaleSet(inputVMScaleSet, listResponse.FirstOrDefault(x => x.Name == vmssName), hasManagedDisks);

                var listSkusResponse = await (VirtualMachineScaleSetsClient.ListSkusAsync(rgName, vmssName)).ToEnumerableAsync();
                Assert.NotNull(listSkusResponse);
                Assert.False(listSkusResponse.Count() == 0);

                if (zones != null)
                {
                    var query = "LatestModelApplied eq true";
                    //query.SetFilter(vm => vm.LatestModelApplied == true);
                    var listVMsResponse =  await (VirtualMachineScaleSetVMsClient.ListAsync(rgName, vmssName, query)).ToEnumerableAsync();
                    Assert.False(listVMsResponse == null, "VMScaleSetVMs not returned");
                    Assert.True(listVMsResponse.Count() == inputVMScaleSet.Sku.Capacity);

                    foreach (var vmScaleSetVM in listVMsResponse)
                    {
                        string instanceId = vmScaleSetVM.InstanceId;
                        var getVMResponse = await VirtualMachineScaleSetVMsClient.GetAsync(rgName, vmssName, instanceId);
                        ValidateVMScaleSetVM(inputVMScaleSet, instanceId, getVMResponse, hasManagedDisks);
                    }
                }

                vmScaleSetValidator?.Invoke(getResponse);

                await VirtualMachineScaleSetsClient.StartDeleteAsync(rgName, vmssName);
            }
            finally
            {
                //Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                //of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }
    }
}
