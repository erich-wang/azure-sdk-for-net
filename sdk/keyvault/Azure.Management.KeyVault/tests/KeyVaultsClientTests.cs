// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.KeyVault.Models;
using NUnit.Framework;

namespace Azure.Management.KeyVault.Tests
{
    public class KeyVaultsClientTests : KeyVaultsManagementClientBase
    {
        public KeyVaultsClientTests(bool isAsync, KeyVaultsManagementClientOption.ServiceVersion serviceVersion)
            : base(isAsync, serviceVersion)
        {
        }

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Client = GetClient();

                //ChallengeBasedAuthenticationPolicy.AuthenticationChallenge.ClearCache();
            }
        }

        [Test]
        public async Task CreateKeyVaults()
        {
            var vaultProperties = new VaultProperties(new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47"), new Sku(SkuName.Standard));
            var properties = new Models.VaultCreateOrUpdateParameters("eastus", vaultProperties);
            var vault = await Client.StartCreateOrUpdateAsync("eriwan_key", "eriwanvault", properties);

            var returnedVault = await Client.GetAsync("eriwan_key", "eriwanvault");

            Assert.AreEqual(vault.Id, returnedVault.Value.Id);
        }
    }
}
