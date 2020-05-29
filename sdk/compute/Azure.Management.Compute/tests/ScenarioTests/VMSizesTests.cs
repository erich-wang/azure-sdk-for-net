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
using Castle.Core.Logging;

namespace Azure.Management.Compute.Tests
{
    public class VMSizesTests :ComputeClientBase
    {
        public VMSizesTests(bool isAsync)
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
        public async Task TestListVMSizes()
        {
            string location = DefaultLocation.Replace(" ", "");

            var virtualMachineSizeListResponse = await (VirtualMachineSizesClient.ListAsync(location)).ToEnumerableAsync();
            Helpers.ValidateVirtualMachineSizeListResponse(virtualMachineSizeListResponse);
        }
    }
}
