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
    public class VMRunCommandsTests : ComputeClientBase
    {
        public VMRunCommandsTests(bool isAsync)
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
        public async Task TestListVMRunCommands()
        {
                string location = DefaultLocation.Replace(" ", "");
                string documentId = "RunPowerShellScript";

                // Verify the List of commands
                var runCommandList = VirtualMachineRunCommandsClient.ListAsync(location);
                var runCommandListResponse = await runCommandList.ToEnumerableAsync();
                Assert.NotNull(runCommandListResponse);
                Assert.True(runCommandListResponse.Count() > 0, "ListRunCommands should return at least 1 command");
                RunCommandDocumentBase documentBase =
                    runCommandListResponse.FirstOrDefault(x => string.Equals(x.Id, documentId));
                Assert.NotNull(documentBase);

                // Verify Get a specific RunCommand
                RunCommandDocument document = await VirtualMachineRunCommandsClient.GetAsync(location, documentId);
                Assert.NotNull(document);
                Assert.NotNull(document.Script);
                Assert.True(document.Script.Count > 0, "Script should contain at least one command.");
                Assert.NotNull(document.Parameters);
                Assert.True(document.Parameters.Count == 2, "Script should have 2 parameters.");
        }
    }
}
