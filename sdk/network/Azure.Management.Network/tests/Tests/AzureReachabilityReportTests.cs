// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Network.Models;
using Azure.Management.Network.Tests.Helpers;
using NUnit.Framework;

namespace Azure.Management.Network.Tests.Tests
{
    public class AzureReachabilityReportTests : NetworkTestsManagementClientBase
    {
        public AzureReachabilityReportTests(bool isAsync) : base(isAsync)
        {
        }

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task AzureReachabilityReportCountryLevelAggregationTest()
        {
            AzureReachabilityReportParameters parameters = new AzureReachabilityReportParameters(
                new AzureReachabilityReportLocation("United States"), Recording.UtcNow.AddDays(-10), Recording.UtcNow.AddDays(-5))
            {
                AzureLocations = new List<string> { "West US" }
            };
            Operation<AzureReachabilityReport> reportOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetAzureReachabilityReportAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AzureReachabilityReport> report = await reportOperation.WaitForCompletionAsync();

            //Validation
            Assert.AreEqual("Country", report.Value.AggregationLevel);
            Assert.AreEqual("United States", report.Value.ProviderLocation.Country);
            Assert.AreEqual("West US", report.Value.ReachabilityReport[0].AzureLocation);
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task AzureReachabilityReportStateLevelAggregationTest()
        {
            AzureReachabilityReportParameters parameters = new AzureReachabilityReportParameters(
                new AzureReachabilityReportLocation("United States") { State = "washington" }, Recording.UtcNow.AddDays(-10), Recording.UtcNow.AddDays(-5))
            {
                AzureLocations = new List<string> { "West US" }
            };
            Operation<AzureReachabilityReport> reportOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetAzureReachabilityReportAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AzureReachabilityReport> report = await reportOperation.WaitForCompletionAsync();

            //Validation
            Assert.AreEqual("State", report.Value.AggregationLevel);
            Assert.AreEqual("United States", report.Value.ProviderLocation.Country);
            Assert.AreEqual("washington", report.Value.ProviderLocation.State);
            Assert.AreEqual("West US", report.Value.ReachabilityReport[0].AzureLocation);
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task AzureReachabilityReportCityLevelAggregationTest()
        {
            AzureReachabilityReportParameters parameters = new AzureReachabilityReportParameters(
                new AzureReachabilityReportLocation("United States") { State = "washington", City = "seattle" }, Recording.UtcNow.AddDays(-10), Recording.UtcNow.AddDays(-5))
            {
                AzureLocations = new List<string> { "West US" }
            };
            Operation<AzureReachabilityReport> reportOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartGetAzureReachabilityReportAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AzureReachabilityReport> report = await reportOperation.WaitForCompletionAsync();

            //Validation
            Assert.AreEqual("City", report.Value.AggregationLevel);
            Assert.AreEqual("United States", report.Value.ProviderLocation.Country);
            Assert.AreEqual("washington", report.Value.ProviderLocation.State);
            Assert.AreEqual("seattle", report.Value.ProviderLocation.City);
            Assert.AreEqual("West US", report.Value.ReachabilityReport[0].AzureLocation);
        }
    }
}
