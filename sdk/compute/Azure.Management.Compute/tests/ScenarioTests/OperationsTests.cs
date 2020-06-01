﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class OperationsTests : VMTestBase
    {
        public OperationsTests(bool isAsync)
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
        //[Trait("Name", "TestCrpOperations")]
        public async Task TestCrpOperations()
        {
            EnsureClientsInitialized();
            var operations =  OperationsClient.ListAsync();
            var operationResult = await operations.ToEnumerableAsync();
            //AzureOperationResponse<IEnumerable<ComputeOperationValue>> operations = OperationsClient.ListWithHttpMessagesAsync().GetAwaiter().GetResult();

            Assert.NotNull(operationResult);
            Assert.IsTrue(operationResult.Count()>=1);
        }
    }
}
