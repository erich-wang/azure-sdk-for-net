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
    public class VMIdentityTests : VMTestBase
    {
        public VMIdentityTests(bool isAsync)
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
        public async Task TestVMIdentitySystemAssignedUserAssigned()
        {
            //
            // Prerequisite: in order to record this test, first create a user identity in resource group 'identitytest' and set the value of identity here.
            //
            const string rgName = "identitytest";
            const string identity = "/subscriptions/24fb23e3-6ba3-41f0-9b6e-e41131d5d61e/resourcegroups/identitytest/providers/Microsoft.ManagedIdentity/userAssignedIdentities/userid";
            EnsureClientsInitialized();

            ImageReference imgageRef = await GetPlatformVMImage(useWindowsImage: true);

            // Create resource group
            string storageAccountName = Recording.GenerateAssetName(TestPrefix);
            string asName = Recording.GenerateAssetName("as");
            VirtualMachine inputVM;
            bool passed = false;

            try
            {
                var storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName);

                Action<VirtualMachine> addUserIdentity = vm =>
                {
                    vm.Identity = new VirtualMachineIdentity();
                    vm.Identity.Type = ResourceIdentityType.SystemAssignedUserAssigned;
                    vm.Identity.UserAssignedIdentities = new Dictionary<string, Components1H8M3EpSchemasVirtualmachineidentityPropertiesUserassignedidentitiesAdditionalproperties>()
                    {
                        { identity, new Components1H8M3EpSchemasVirtualmachineidentityPropertiesUserassignedidentitiesAdditionalproperties() }
                    };
                };

                var returnTwoVM = await CreateVM(rgName, asName, storageAccountOutput, imgageRef , addUserIdentity);
                VirtualMachine vmResult = returnTwoVM.Item1;
                inputVM = returnTwoVM.Item2;
                Assert.AreEqual(ResourceIdentityType.SystemAssignedUserAssigned, vmResult.Identity.Type);
                Assert.NotNull(vmResult.Identity.PrincipalId);
                Assert.NotNull(vmResult.Identity.TenantId);
                Assert.True(vmResult.Identity.UserAssignedIdentities.Keys.Contains(identity));
                Assert.NotNull(vmResult.Identity.UserAssignedIdentities[identity].PrincipalId);
                Assert.NotNull(vmResult.Identity.UserAssignedIdentities[identity].ClientId);

                var getVM = (await VirtualMachinesClient.GetAsync(rgName, inputVM.Name)).Value;
                Assert.AreEqual(ResourceIdentityType.SystemAssignedUserAssigned, getVM.Identity.Type);
                Assert.NotNull(getVM.Identity.PrincipalId);
                Assert.NotNull(getVM.Identity.TenantId);
                Assert.True(getVM.Identity.UserAssignedIdentities.Keys.Contains(identity));
                Assert.NotNull(getVM.Identity.UserAssignedIdentities[identity].PrincipalId);
                Assert.NotNull(getVM.Identity.UserAssignedIdentities[identity].ClientId);

                await VirtualMachinesClient.StartDeleteAsync(rgName, inputVM.Name);

                passed = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Assert.True(passed);
            }
        }
    }
}
