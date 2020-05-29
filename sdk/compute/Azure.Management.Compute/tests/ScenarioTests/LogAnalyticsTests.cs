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
using Azure.Management.Storage.Models;

namespace Azure.Management.Compute.Tests
{
    public class LogAnalyticsTests : VMTestBase
    {
        public LogAnalyticsTests(bool isAsync)
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
        public async Task TestExportingThrottlingLogs()
        {
                string rg1Name = Recording.GenerateAssetName(TestPrefix);

                string storageAccountName = Recording.GenerateAssetName(TestPrefix);

                try
                {
                    EnsureClientsInitialized();

                    string sasUri = await GetBlobContainerSasUri(rg1Name, storageAccountName);

                    RequestRateByIntervalInput requestRateByIntervalInput = new RequestRateByIntervalInput(sasUri, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-8), IntervalInMins.FiveMins);

                    var result = await LogAnalyticsClient.StartExportRequestRateByIntervalAsync("westcentralus" ,requestRateByIntervalInput);
                    //BUG: LogAnalytics API does not return correct result.
                    //Assert.EndsWith(".csv", result.Properties.Output);

                    ThrottledRequestsInput throttledRequestsInput = new ThrottledRequestsInput(sasUri, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-8))
                    {
                        GroupByOperationName = true,
                    };

                    var result1= await LogAnalyticsClient.StartExportThrottledRequestsAsync("westcentralus" ,throttledRequestsInput);

                    //BUG: LogAnalytics API does not return correct result.
                    //Assert.EndsWith(".csv", result.Properties.Output);
                }
                finally
                {
                    await ResourceGroupsClient.StartDeleteAsync(rg1Name);
                }
        }

        private async Task<string> GetBlobContainerSasUri(string rg1Name, string storageAccountName)
        {
            string sasUri = "foobar";

            if (Mode == RecordedTestMode.Record)
            {
                StorageAccount storageAccountOutput = await CreateStorageAccount(rg1Name, storageAccountName);
                var accountKeyResult = (await StorageAccountsClient.ListKeysAsync(rg1Name, storageAccountName)).Value;
                //var accountKeyResult = await StorageAccountsClient.ListKeysWithHttpMessagesAsync(rg1Name, storageAccountName).Result;
                StorageAccount storageAccount = new StorageAccount(DefaultLocation);

                BlobContainer container = await BlobContainersClient.GetAsync(rg1Name, storageAccountName, "sascontainer");
                //container.CreateIfNotExistsAsync();
                sasUri =  GetContainerSasUri(container);
            }

            return sasUri;
        }

        private string GetContainerSasUri(BlobContainer container)
        {
            //SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            //sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddDays(-1);
            //sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddDays(2);
            //sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            ////Generate the shared access signature on the blob, setting the constraints directly on the signature.
            //string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            ////Return the URI string for the container, including the SAS token.
            //return container.Uri + sasContainerToken;
            return "just a url";
        }
    }
}
