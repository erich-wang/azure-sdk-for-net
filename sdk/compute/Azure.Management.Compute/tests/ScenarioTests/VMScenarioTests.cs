﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class VMScenarioTests : VMTestBase
    {
        public VMScenarioTests(bool isAsync)
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
        //[Trait("Name", "TestVMScenarioOperations")]
        public async Task TestVMScenarioOperations()
        {
            await TestVMScenarioOperationsInternal("TestVMScenarioOperations");
        }
        /// <summary>
        /// Covers following Operations for managed disks:
        /// Create RG
        /// Create Network Resources
        /// Create VM with WriteAccelerator enabled OS and Data disk
        /// GET VM Model View
        /// GET VM InstanceView
        /// GETVMs in a RG
        /// List VMSizes in a RG
        /// List VMSizes in an AvailabilitySet
        /// Delete RG
        ///
        /// To record this test case, you need to run it in region which support XMF VMSizeFamily like eastus2.
        /// </summary>
        [Test]
        [Ignore("this should be tested by generate team")]
        //[Trait("Name", "TestVMScenarioOperations_ManagedDisks")]
        public async Task TestVMScenarioOperations_ManagedDisks()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_ManagedDisks", vmSize: VirtualMachineSizeTypes.StandardM64S.ToString(), hasManagedDisks: true,
                    osDiskStorageAccountType: StorageAccountTypes.PremiumLRS.ToString(), dataDiskStorageAccountType: StorageAccountTypes.PremiumLRS.ToString(), writeAcceleratorEnabled: true);
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
        //[Trait("Name", "TestVMScenarioOperations_DiffDisks")]
        public async Task TestVMScenarioOperations_DiffDisks()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "northeurope");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_DiffDisks", vmSize: VirtualMachineSizeTypes.StandardDS148V2.ToString(), hasManagedDisks: true,
                   hasDiffDisks: true, osDiskStorageAccountType: StorageAccountTypes.StandardLRS.ToString());
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
        //[Trait("Name", "TestVMScenarioOperations_ManagedDisks_DiskEncryptionSet")]
        public async Task TestVMScenarioOperations_ManagedDisks_DiskEncryptionSet()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");

            string diskEncryptionSetId = getDefaultDiskEncryptionSetId();
            try
            {
                //change location centraluseuap to southeastasia
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "southeastasia");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_ManagedDisks_DiskEncryptionSet", vmSize: VirtualMachineSizeTypes.StandardA1V2.ToString(), hasManagedDisks: true,
                   osDiskStorageAccountType: StorageAccountTypes.StandardLRS.ToString(), diskEncryptionSetId: diskEncryptionSetId);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// TODO: StandardSSD is currently in preview and is available only in a few regions. Once it goes GA, it can be tested in
        /// the default test location.
        /// </summary>
        [Test]
        ////[Trait("Name", "TestVMScenarioOperations_ManagedDisks_StandardSSD")]
        public async Task TestVMScenarioOperations_ManagedDisks_StandardSSD()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "northeurope");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_ManagedDisks_StandardSSD", hasManagedDisks: true,
                    osDiskStorageAccountType: StorageAccountTypes.StandardSSDLRS.ToString(), dataDiskStorageAccountType: StorageAccountTypes.StandardSSDLRS.ToString());
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it in zone supported regions like eastus2.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScenarioOperations_ManagedDisks_PirImage_Zones")]
        public async Task TestVMScenarioOperations_ManagedDisks_PirImage_Zones()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "centralus");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_ManagedDisks_PirImage_Zones", hasManagedDisks: true, zones: new List<string> { "1" }, callUpdateVM: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it in zone supported regions like eastus2euap.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScenarioOperations_ManagedDisks_UltraSSD")]
        public async Task TestVMScenarioOperations_ManagedDisks_UltraSSD()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_ManagedDisks_UltraSSD", hasManagedDisks: true, zones: new List<string> { "1" },
                    vmSize: VirtualMachineSizeTypes.StandardE16SV3.ToString(), osDiskStorageAccountType: StorageAccountTypes.PremiumLRS.ToString(),
                    dataDiskStorageAccountType: StorageAccountTypes.UltraSSDLRS.ToString(), callUpdateVM: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        /// <summary>
        /// To record this test case, you need to run it in zone supported regions like eastus2euap.
        /// </summary>
        [Test]
        //[Trait("Name", "TestVMScenarioOperations_PpgScenario")]
        public async Task TestVMScenarioOperations_PpgScenario()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            try
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", "eastus2");
                await TestVMScenarioOperationsInternal("TestVMScenarioOperations_PpgScenario", hasManagedDisks: true, isPpgScenario: true);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        private async Task TestVMScenarioOperationsInternal(string methodName, bool hasManagedDisks = false, IList<string> zones = null, string vmSize = "Standard_A0",
            string osDiskStorageAccountType = "Standard_LRS", string dataDiskStorageAccountType = "Standard_LRS", bool? writeAcceleratorEnabled = null,
            bool hasDiffDisks = false, bool callUpdateVM = false, bool isPpgScenario = false, string diskEncryptionSetId = null)
        {
            EnsureClientsInitialized();

            var imageRef = await GetPlatformVMImage(useWindowsImage: true);
            const string expectedOSName = "Windows Server 2012 R2 Datacenter", expectedOSVersion = "Microsoft Windows NT 6.3.9600.0", expectedComputerName = ComputerName;
            // Create resource group
            var rgName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            string ppgName = null;
            string expectedPpgReferenceId = null;

            if (isPpgScenario)
            {
                ppgName = Recording.GenerateAssetName("ppgtest");
                expectedPpgReferenceId = Helpers.GetProximityPlacementGroupRef(m_subId, rgName, ppgName);
            }

            VirtualMachine inputVM;
            try
            {
                if (!hasManagedDisks)
                {
                    await CreateStorageAccount(rgName, storageAccountName);
                }

                var returnTwoVM=  await CreateVM(rgName, asName, storageAccountName, imageRef, hasManagedDisks: hasManagedDisks, hasDiffDisks: hasDiffDisks, vmSize: vmSize, osDiskStorageAccountType: osDiskStorageAccountType,
                    dataDiskStorageAccountType: dataDiskStorageAccountType, writeAcceleratorEnabled: writeAcceleratorEnabled, zones: zones, ppgName: ppgName, diskEncryptionSetId: diskEncryptionSetId);
                //VirtualMachine outVM = returnTwoVM.Item1;
                inputVM = returnTwoVM.Item2;
                // Instance view is not completely populated just after VM is provisioned. So we wait here for a few minutes to
                // allow GA blob to populate.
                WaitMinutes(5);

                var getVMWithInstanceViewResponse =  (await VirtualMachinesClient.GetAsync(rgName, inputVM.Name)).Value;
                Assert.True(getVMWithInstanceViewResponse != null, "VM in Get");

                if (diskEncryptionSetId != null)
                {

                    Assert.True(getVMWithInstanceViewResponse.StorageProfile.OsDisk.ManagedDisk.DiskEncryptionSet != null, "OsDisk.ManagedDisk.DiskEncryptionSet is null");
                    Assert.True(string.Equals(diskEncryptionSetId, getVMWithInstanceViewResponse.StorageProfile.OsDisk.ManagedDisk.DiskEncryptionSet.Id, StringComparison.OrdinalIgnoreCase),
                        "OsDisk.ManagedDisk.DiskEncryptionSet.Id is not matching with expected DiskEncryptionSet resource");

                    Assert.AreEqual(1, getVMWithInstanceViewResponse.StorageProfile.DataDisks.Count);
                    Assert.True(getVMWithInstanceViewResponse.StorageProfile.DataDisks[0].ManagedDisk.DiskEncryptionSet != null, ".DataDisks.ManagedDisk.DiskEncryptionSet is null");
                    Assert.True(string.Equals(diskEncryptionSetId, getVMWithInstanceViewResponse.StorageProfile.DataDisks[0].ManagedDisk.DiskEncryptionSet.Id, StringComparison.OrdinalIgnoreCase),
                        "DataDisks.ManagedDisk.DiskEncryptionSet.Id is not matching with expected DiskEncryptionSet resource");
                }

                ValidateVMInstanceView(inputVM, getVMWithInstanceViewResponse, hasManagedDisks, expectedComputerName, expectedOSName, expectedOSVersion);

                var getVMInstanceViewResponse = await VirtualMachinesClient.InstanceViewAsync(rgName, inputVM.Name);
                Assert.True(getVMInstanceViewResponse != null, "VM in InstanceView");
                ValidateVMInstanceView(inputVM, getVMInstanceViewResponse, hasManagedDisks, expectedComputerName, expectedOSName, expectedOSVersion);

                bool hasUserDefinedAS = zones == null;

                string expectedVMReferenceId = Helpers.GetVMReferenceId(m_subId, rgName, inputVM.Name);
                var listResponse = await (VirtualMachinesClient.ListAsync(rgName)).ToEnumerableAsync();
                ValidateVM(inputVM, listResponse.FirstOrDefault(x => x.Name == inputVM.Name),
                    expectedVMReferenceId, hasManagedDisks, hasUserDefinedAS, writeAcceleratorEnabled, hasDiffDisks, expectedPpgReferenceId: expectedPpgReferenceId);

                var listVMSizesResponse = await (VirtualMachinesClient.ListAvailableSizesAsync(rgName, inputVM.Name)).ToEnumerableAsync();
                Helpers.ValidateVirtualMachineSizeListResponse(listVMSizesResponse, hasAZ: zones != null, writeAcceleratorEnabled: writeAcceleratorEnabled, hasDiffDisks: hasDiffDisks);

                listVMSizesResponse = await (AvailabilitySetsClient.ListAvailableSizesAsync(rgName, asName)).ToEnumerableAsync();
                Helpers.ValidateVirtualMachineSizeListResponse(listVMSizesResponse, hasAZ: zones != null, writeAcceleratorEnabled: writeAcceleratorEnabled, hasDiffDisks: hasDiffDisks);

                if (isPpgScenario)
                {
                    ProximityPlacementGroup outProximityPlacementGroup = await ProximityPlacementGroupsClient.GetAsync(rgName, ppgName);
                    string expectedAvSetReferenceId = Helpers.GetAvailabilitySetRef(m_subId, rgName, asName);
                    Assert.AreEqual(1, outProximityPlacementGroup.VirtualMachines.Count);
                    Assert.AreEqual(1, outProximityPlacementGroup.AvailabilitySets.Count);
                    Assert.AreEqual(expectedVMReferenceId, outProximityPlacementGroup.VirtualMachines.First().Id);
                    Assert.AreEqual(expectedAvSetReferenceId, outProximityPlacementGroup.AvailabilitySets.First().Id);
                    //Assert.Equal(expectedVMReferenceId, outProximityPlacementGroup.VirtualMachines.First().Id, StringComparer.OrdinalIgnoreCase);
                    //Assert.Equal(expectedAvSetReferenceId, outProximityPlacementGroup.AvailabilitySets.First().Id, StringComparer.OrdinalIgnoreCase);
                }

                if (callUpdateVM)
                {
                    VirtualMachineUpdate updateParams = new VirtualMachineUpdate()
                    {
                        Tags = inputVM.Tags
                    };

                    string updateKey = "UpdateTag";
                    updateParams.Tags.Add(updateKey, "UpdateTagValue");
                    VirtualMachine updateResponse = await (await VirtualMachinesClient.StartUpdateAsync(rgName, inputVM.Name, updateParams)).WaitForCompletionAsync();

                    Assert.True(updateResponse.Tags.ContainsKey(updateKey));
                }
            }
            finally
            {
                // Fire and forget. No need to wait for RG deletion completion
                try
                {
                    await ResourceGroupsClient.StartDeleteAsync(rgName);
                }
                catch (Exception e)
                {
                    // Swallow this exception so that the original exception is thrown
                    Console.WriteLine(e);
                }
            }
        }
    }
}
