﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Tests;
using NUnit.Framework;

namespace ResourceGroups.Tests
{
    public class LiveProviderTests : ResourceOperationsTestsBase
    {
        public LiveProviderTests(bool isAsync)
            : base(isAsync)
        {
        }

        //IOTHub
        private const string ProviderName = "Microsoft.Scheduler";

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        public async Task ProviderGetValidateMessage()
        {
            var reg = await ProvidersClient.RegisterAsync(ProviderName);
            Assert.NotNull(reg);

            var result = (await ProvidersClient.GetAsync(ProviderName)).Value;

            // Validate headers
            //Assert.AreEqual(HttpMethod.Get, handler.Method);
            //Assert.NotNull(handler.RequestHeaders.GetValues("Authorization"));

            // Validate result
            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Id);
            Assert.AreEqual(ProviderName, result.Namespace);
            Assert.True("Registered" == result.RegistrationState ||
                "Registering" == result.RegistrationState,
                string.Format("Provider registration state was not 'Registered' or 'Registering', instead it was '{0}'", result.RegistrationState));
            Assert.IsNotEmpty(result.ResourceTypes);
            Assert.IsNotEmpty(result.ResourceTypes[0].Locations);
        }

        [Test]
        public async Task ProviderListValidateMessage()
        {
            var reg = await ProvidersClient.RegisterAsync(ProviderName);
            Assert.NotNull(reg);

            var result = await ProvidersClient.ListAsync(null).ToEnumerableAsync();

            // Validate headers
            //Assert.Equal(HttpMethod.Get, handler.Method);
            //Assert.NotNull(handler.RequestHeaders.GetValues("Authorization"));

            // Validate result
            Assert.True(result.Any());
            var websiteProvider =
                result.First(
                    p => p.Namespace.Equals(ProviderName, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(ProviderName, websiteProvider.Namespace);
            Assert.True("Registered" == websiteProvider.RegistrationState ||
                "Registering" == websiteProvider.RegistrationState,
                string.Format("Provider registration state was not 'Registered' or 'Registering', instead it was '{0}'", websiteProvider.RegistrationState));
            Assert.IsNotEmpty(websiteProvider.ResourceTypes);
            Assert.IsNotEmpty(websiteProvider.ResourceTypes[0].Locations);
        }

        [Test]
        public async Task GetProviderWithAliases()
        {
            var computeNamespace = "Microsoft.Compute";

            var reg = await ProvidersClient.RegisterAsync(computeNamespace);
            Assert.NotNull(reg);

            var result = await ProvidersClient.ListAsync(expand: "resourceTypes/aliases").ToEnumerableAsync();

            // Validate headers
            //Assert.Equal(HttpMethod.Get, handler.Method);
            //Assert.NotNull(handler.RequestHeaders.GetValues("Authorization"));

            // Validate result
            Assert.True(result.Any());
            var computeProvider = result.First(
                provider => string.Equals(provider.Namespace, computeNamespace, StringComparison.OrdinalIgnoreCase));

            Assert.IsNotEmpty(computeProvider.ResourceTypes);
            var virtualMachinesType = computeProvider.ResourceTypes.First(
                resourceType => string.Equals(resourceType.ResourceType, "virtualMachines", StringComparison.OrdinalIgnoreCase));

            Assert.IsNotEmpty(virtualMachinesType.Aliases);
            Assert.AreEqual("Microsoft.Compute/licenseType", virtualMachinesType.Aliases[0].Name);
            Assert.AreEqual("properties.licenseType", virtualMachinesType.Aliases[0].Paths[0].Path);

            computeProvider = (await ProvidersClient.GetAsync(resourceProviderNamespace: computeNamespace, expand: "resourceTypes/aliases")).Value;

            Assert.IsNotEmpty(computeProvider.ResourceTypes);
            virtualMachinesType = computeProvider.ResourceTypes.First(
                resourceType => string.Equals(resourceType.ResourceType, "virtualMachines", StringComparison.OrdinalIgnoreCase));

            Assert.IsNotEmpty(virtualMachinesType.Aliases);
            Assert.AreEqual("Microsoft.Compute/licenseType", virtualMachinesType.Aliases[0].Name);
            Assert.AreEqual("properties.licenseType", virtualMachinesType.Aliases[0].Paths[0].Path);
        }

        [Test]
        public async Task VerifyProviderRegister()
        {
            await ProvidersClient.RegisterAsync(ProviderName);

            var provider = (await ProvidersClient.GetAsync(ProviderName)).Value;
            Assert.True(provider.RegistrationState == "Registered" ||
                        provider.RegistrationState == "Registering");
        }

        [Test]
        public async Task VerifyProviderUnregister()
        {
            var registerResult = await ProvidersClient.RegisterAsync(ProviderName);

            var provider = (await ProvidersClient.GetAsync(ProviderName)).Value;
            Assert.True(provider.RegistrationState == "Registered" ||
                        provider.RegistrationState == "Registering");

            var unregisterResult = await ProvidersClient.UnregisterAsync(ProviderName);

            provider = (await ProvidersClient.GetAsync(ProviderName)).Value;
            Assert.True(provider.RegistrationState == "NotRegistered" ||
                        provider.RegistrationState == "Unregistering",
                        "RegistrationState is expected NotRegistered or Unregistering. Actual value " +
                        provider.RegistrationState);
        }
    }
}
