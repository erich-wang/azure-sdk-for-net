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
    public class VMOperationalTests : VMTestBase
    {
        public VMOperationalTests(bool isAsync)
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
        public class Image
        {
            //[JsonProperty("uri")]
            public string Uri { get; set; }
        }

        public class OSDisk
        {
            //[JsonProperty("image")]
            public Image Image { get; set; }
        }

        public class StorageProfile
        {
            //[JsonProperty("osDisk")]
            public OSDisk OSDisk { get; set; }
        }

        public class Properties
        {
            //[JsonProperty("storageProfile")]
            public StorageProfile StorageProfile { get; set; }
        }

        public class Resource
        {
            //[JsonProperty("properties")]
            public Properties Properties { get; set; }
        }

        public class Template
        {
            //[JsonProperty("resources")]
            public List<Resource> Resources { get; set; }
        }

        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VM
        /// GET VM Model View
        /// Start VM
        /// Stop VM
        /// Restart VM
        /// RunCommand VM
        /// Deallocate VM
        /// Generalize VM
        /// Capture VM
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMOperations()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string as1Name = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1;

            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, as1Name, storageAccountOutput, imageRef);
                var vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                await (await VirtualMachinesClient.StartStartAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                await (await VirtualMachinesClient.StartRedeployAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                await (await VirtualMachinesClient.StartRestartAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();

                var runCommandImput = new RunCommandInput("RunPowerShellScript")
                {
                    Script = new List<string>() {
                            "param(",
                            "    [string]$arg1,",
                            "    [string]$arg2",
                            ")",
                            "echo This is a sample script with parameters $arg1 $arg2"
                        },
                    Parameters = new List<RunCommandInputParameter>()
                        {
                            new RunCommandInputParameter("arg1","value1"),
                            new RunCommandInputParameter("arg2","value2"),
                        }
                };
                RunCommandResult result = (await (await VirtualMachinesClient.StartRunCommandAsync(rg1Name, vm1.Name, runCommandImput)).WaitForCompletionAsync()).Value;
                Assert.NotNull(result);
                Assert.NotNull(result.Value);
                Assert.True(result.Value.Count > 0);

                await (await VirtualMachinesClient.StartPowerOffAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                await (await VirtualMachinesClient.StartDeallocateAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                await VirtualMachinesClient.GeneralizeAsync(rg1Name, vm1.Name);

                VirtualMachine ephemeralVM;
                string as2Name = as1Name + "_ephemeral";
                var returnTwoVM= await CreateVM(rg1Name, as2Name, storageAccountName, imageRef, hasManagedDisks: true, hasDiffDisks: true, vmSize: VirtualMachineSizeTypes.StandardDS5V2.ToString(),
                    osDiskStorageAccountType: StorageAccountTypes.StandardLRS.ToString(), dataDiskStorageAccountType: StorageAccountTypes.StandardLRS.ToString());
                ephemeralVM = returnTwoVM.Item2;
                await (await VirtualMachinesClient.StartReimageAsync(rg1Name, ephemeralVM.Name)).WaitForCompletionAsync();
                //await VirtualMachinesClient.StartReimageAsync(rg1Name, ephemeralVM.Name, tempDisk: true);
                var captureParams = new VirtualMachineCaptureParameters(Recording.GenerateAssetName(TestPrefix), Recording.GenerateAssetName(TestPrefix), true);

                var captureResponse = await (await VirtualMachinesClient.StartCaptureAsync(rg1Name, vm1.Name, captureParams)).WaitForCompletionAsync();

                Assert.NotNull(captureResponse);
                Assert.True(captureResponse.Value.Resources.Count > 0);
                string resource = captureResponse.Value.Resources[0].ToString();
                bool containResource1 = false;
                foreach (var resourceDetail in resource.ToLowerInvariant())
                {
                    if (resourceDetail.ToString() == captureParams.DestinationContainerName.ToLowerInvariant())
                    {
                        containResource1 = true;
                        break;
                    }
                }
                bool containResource2 = false;
                foreach (var resourceDetail in resource.ToLowerInvariant())
                {
                    if (resourceDetail.ToString() == captureParams.VhdPrefix.ToLowerInvariant())
                    {
                        containResource2 = true;
                        break;
                    }
                }
                Assert.True(containResource1);
                Assert.True(containResource2);

                //Assert.Contains(captureParams.DestinationContainerName.ToLowerInvariant(), resource.ToLowerInvariant());
                //Assert.Contains(captureParams.VhdPrefix.ToLowerInvariant(), resource.ToLowerInvariant());

                Resource template = JsonSerializer.Deserialize<Resource>(resource);
                string imageUri = template.Properties.StorageProfile.OSDisk.Image.Uri;
                Assert.False(string.IsNullOrEmpty(imageUri));

                // Create 3rd VM from the captured image
                // TODO : Provisioning Time-out Issues
                VirtualMachine inputVM2;
                string as3Name = as1Name + "b";
                returnTwovm = await CreateVM(rg1Name, as3Name, storageAccountOutput, imageRef,
                    vm =>
                    {
                        vm.StorageProfile.ImageReference = null;
                        vm.StorageProfile.OsDisk.Image = new VirtualHardDisk { Uri = imageUri };
                        vm.StorageProfile.OsDisk.Vhd.Uri = vm.StorageProfile.OsDisk.Vhd.Uri.Replace(".vhd", "copy.vhd");
                        vm.StorageProfile.OsDisk.OsType = OperatingSystemTypes.Windows;
                    }, false, false);
                var vm3 = returnTwovm.Item1;
                inputVM2 = returnTwovm.Item2;
                Assert.True(vm3.StorageProfile.OsDisk.Image.Uri == imageUri);
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
            }
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VM
        /// GET VM Model View
        /// Redeploy VM
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMOperations_Redeploy()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1;

            bool passed = false;
            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, asName, storageAccountOutput, imageRef);
                VirtualMachine vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                var redeployOperationResponse = await (await VirtualMachinesClient.StartRedeployAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                //.BeginRedeployWithHttpMessagesAsync
                //Assert.Equal(HttpStatusCode.Accepted, redeployOperationResponse.Result.Response.StatusCode);
                var lroResponse = await (await VirtualMachinesClient.StartRedeployAsync(rg1Name,
                    vm1.Name)).WaitForCompletionAsync();
                //var lroResponse = await VirtualMachinesClient.StartRedeployAsync(rg1Name,
                //    vm1.Name).GetAwaiter().GetResult();
                //Assert.Equal(ComputeOperationStatus.Succeeded, lroResponse.Status);

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                var deleteRg1Response = await ResourceGroupsClient.StartDeleteAsync(rg1Name);
                //var deleteRg1Response = await ResourceGroups.BeginDeleteWithHttpMessagesAsync(rg1Name);
                //Assert.True(deleteRg1Response.StatusCode == HttpStatusCode.Accepted,
                //    "BeginDeleting status was not Accepted.");
            }

            Assert.True(passed);
        }
        [Test]
        public async Task TestVMOperations_Reapply()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1;

            bool passed = false;
            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, asName, storageAccountOutput, imageRef);
                VirtualMachine vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                var reapplyperationResponse = await (await VirtualMachinesClient.StartReapplyAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                var lroResponse = await (await VirtualMachinesClient.StartReapplyAsync(rg1Name,
                    vm1.Name)).WaitForCompletionAsync();
                //var lroResponse =  await VirtualMachinesClient.StartReapplyAsync(rg1Name,
                //    vm1.Name).GetAwaiter().GetResult();

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                var deleteRg1Response = await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
                //var deleteRg1Response = await ResourceGroupsClient.BeginDeleteWithHttpMessagesAsync(rg1Name);
            }

            Assert.True(passed);
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create VM
        /// Start VM
        /// Shutdown VM with skipShutdown = true
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMOperations_PowerOffWithSkipShutdown()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1;

            bool passed = false;
            try
            {
                // Create Storage Account for this VM
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, asName, storageAccountOutput, imageRef);
                VirtualMachine vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                await (await VirtualMachinesClient.StartStartAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                // Shutdown VM with SkipShutdown = true
                await (await VirtualMachinesClient.StartPowerOffAsync(rg1Name, vm1.Name, true)).WaitForCompletionAsync();

                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                var deleteRg1Response = await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
            }

            Assert.True(passed);
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VM
        /// GET VM Model View
        /// PerformMaintenance VM
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMOperations_PerformMaintenance()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1 = null;

            bool passed = false;
            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwovm = await CreateVM(rg1Name, asName, storageAccountOutput, imageRef);
                VirtualMachine vm1 = returnTwovm.Item1;
                inputVM1 = returnTwovm.Item2;
                await (await VirtualMachinesClient.StartPerformMaintenanceAsync(rg1Name, vm1.Name)).WaitForCompletionAsync();
                passed = true;

            }
            catch (Exception cex)
            {
                passed = true;
                string expectedMessage = $"Operation 'performMaintenance' is not allowed on VM '{inputVM1.Name}' since the Subscription of this VM" +
                    " is not eligible.";
                Assert.AreEqual(expectedMessage, cex.Message);
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                var deleteRg1Response = await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
            }
            Assert.True(passed);
        }
        /// <summary>
        /// Covers following Operations:
        /// Create RG
        /// Create Storage Account
        /// Create Network Resources
        /// Create VM
        /// Call SimulateEviction on that VM
        /// Delete RG
        /// </summary>
        [Test]
        public async Task TestVMOperations_SimulateEviction()
        {
            EnsureClientsInitialized();

            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string rg1Name = Recording.GenerateAssetName(TestPrefix) + 1;
            string asName = Recording.GenerateAssetName("as");
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            VirtualMachine inputVM1;

            bool passed = false;
            try
            {
                // Create Storage Account, so that both the VMs can share it
                var storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);

                var returnTwoVM = await CreateVM(rg1Name,
                                               asName,
                                               storageAccountOutput.Name,
                                               imageRef,
                                               (virtualMachine) =>
                                               {
                                                   virtualMachine.Priority = VirtualMachinePriorityTypes.Spot;
                                                   virtualMachine.AvailabilitySet = null;
                                                   virtualMachine.BillingProfile = new BillingProfile { MaxPrice = -1 };
                                               },
                                               vmSize: VirtualMachineSizeTypes.StandardA1.ToString());
                VirtualMachine vm1 = returnTwoVM.Item1;
                inputVM1 = returnTwoVM.Item2;


                await VirtualMachinesClient.SimulateEvictionAsync(rg1Name, vm1.Name);
                passed = true;
            }
            finally
            {
                // Cleanup the created resources. But don't wait since it takes too long, and it's not the purpose
                // of the test to cover deletion. CSM does persistent retrying over all RG resources.
                var deleteRg1Response = await (await ResourceGroupsClient.StartDeleteAsync(rg1Name)).WaitForCompletionAsync();
            }
            Assert.True(passed);
        }
    }
}
