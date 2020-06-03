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
    public class AvailableProvidersListTests : NetworkTestsManagementClientBase
    {
        public AvailableProvidersListTests(bool isAsync) : base(isAsync)
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
        public async Task AvailableProvidersListAzureLocationCountrySpecifiedTest()
        {
            AvailableProvidersListParameters parameters = new AvailableProvidersListParameters
            {
                AzureLocations = new List<string> { "West US" },
                Country = "United States"
            };
            Operation<AvailableProvidersList> providersListOperation =
                await NetworkManagementClient.GetNetworkWatchersClient().StartListAvailableProvidersAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AvailableProvidersList> providersList = await providersListOperation.WaitForCompletionAsync();
            Assert.AreEqual("United States", providersList.Value.Countries[0].CountryName);
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task AvailableProvidersListAzureLocationCountryStateSpecifiedTest()
        {
            AvailableProvidersListParameters parameters = new AvailableProvidersListParameters
            {
                AzureLocations = new List<string> { "West US" },
                Country = "United States",
                State = "washington"
            };
            Operation<AvailableProvidersList> providersListOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartListAvailableProvidersAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AvailableProvidersList> providersList = await providersListOperation.WaitForCompletionAsync();
            Assert.AreEqual("United States", providersList.Value.Countries[0].CountryName);
            Assert.AreEqual("washington", providersList.Value.Countries[0].States[0].StateName);
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task AvailableProvidersListAzureLocationCountryStateCitySpecifiedTest()
        {
            AvailableProvidersListParameters parameters = new AvailableProvidersListParameters
            {
                AzureLocations = new List<string> { "West US" },
                Country = "United States",
                State = "washington",
                City = "seattle"
            };
            Operation<AvailableProvidersList> providersListOperation = await NetworkManagementClient.GetNetworkWatchersClient().StartListAvailableProvidersAsync("NetworkWatcherRG", "NetworkWatcher_westus", parameters);
            Response<AvailableProvidersList> providersList = await providersListOperation.WaitForCompletionAsync();
            Assert.AreEqual("United States", providersList.Value.Countries[0].CountryName);
            Assert.AreEqual("washington", providersList.Value.Countries[0].States[0].StateName);
            Assert.AreEqual("seattle", providersList.Value.Countries[0].States[0].Cities[0].CityName);
        }
    }
}
