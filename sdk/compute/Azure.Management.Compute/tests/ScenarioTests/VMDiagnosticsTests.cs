// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class VMDiagnosticsTests : VMTestBase
    {
        public VMDiagnosticsTests(bool isAsync)
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
        //[Trait("Name", "TestVMBootDiagnostics")]
        public async Task TestVMBootDiagnostics()
        {

            EnsureClientsInitialized();

            ImageReference imageReference = await GetPlatformVMImage(useWindowsImage: true);
            string resourceGroupName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountForDisksName = Recording.GenerateAssetName(TestPrefix);
            string storageAccountForBootDiagnosticsName = Recording.GenerateAssetName(TestPrefix);
            string availabilitySetName = Recording.GenerateAssetName(TestPrefix);

            try
            {
                StorageAccount storageAccountForDisks = await CreateStorageAccount(resourceGroupName, storageAccountForDisksName);
                StorageAccount storageAccountForBootDiagnostics = await CreateStorageAccount(resourceGroupName, storageAccountForBootDiagnosticsName);

                VirtualMachine inputVM;
                var returnTwoVm= await CreateVM(resourceGroupName, availabilitySetName, storageAccountForDisks, imageReference,
                    (vm) =>
                    {
                        vm.DiagnosticsProfile = GetDiagnosticsProfile(storageAccountForBootDiagnosticsName);
                    });
                inputVM = returnTwoVm.Item2;
                VirtualMachine getVMWithInstanceViewResponse = await VirtualMachinesClient.GetAsync(resourceGroupName, inputVM.Name);
                ValidateVMInstanceView(inputVM, getVMWithInstanceViewResponse);
                ValidateBootDiagnosticsInstanceView(getVMWithInstanceViewResponse.InstanceView.BootDiagnostics, hasError: false);

                // Make boot diagnostics encounter an error due to a missing boot diagnostics storage account
                await VirtualMachinesClient.StartDeallocateAsync(resourceGroupName, inputVM.Name);
                await StorageAccountsClient.DeleteAsync(resourceGroupName, storageAccountForBootDiagnosticsName);
                //await StorageAccountsClient.DeleteWithHttpMessagesAsync(resourceGroupName, storageAccountForBootDiagnosticsName).GetAwaiter().GetResult();
                await WaitForCompletionAsync(await VirtualMachinesClient.StartStartAsync(resourceGroupName, inputVM.Name));

                getVMWithInstanceViewResponse = await VirtualMachinesClient.GetAsync(resourceGroupName, inputVM.Name);
                ValidateBootDiagnosticsInstanceView(getVMWithInstanceViewResponse.InstanceView.BootDiagnostics, hasError: true);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(resourceGroupName);
            }

        }
    }
}
