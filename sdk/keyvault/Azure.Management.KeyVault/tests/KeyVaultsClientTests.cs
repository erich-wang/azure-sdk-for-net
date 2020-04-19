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
            var tenantId = new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47");
            var resGroup = "eriwan_key";
            var vaultName = "eriwanvault6";
            //Need to change to object id of your account
            var userObjectId = "1cf5bc65-5ac8-4dd9-bd98-ed372e7e69be";
            var vaultProperties = new VaultProperties(tenantId, new Sku(SkuName.Standard));
            var accessPolicy = new AccessPolicyEntry(tenantId, userObjectId,
                new Permissions
                {
                    Keys = new KeyPermissions[] { KeyPermissions.Create, KeyPermissions.Delete, KeyPermissions.List },
                    Secrets = new SecretPermissions[] { SecretPermissions.Set, SecretPermissions.Delete, SecretPermissions.List },
                    Certificates = new CertificatePermissions[] { CertificatePermissions.Create, CertificatePermissions.Delete, CertificatePermissions.List },
                });
            vaultProperties.AccessPolicies = new[] { accessPolicy };
            vaultProperties.EnableSoftDelete = false;
            var properties = new Models.VaultCreateOrUpdateParameters("eastus", vaultProperties);
            var vault = await Client.StartCreateOrUpdateAsync(resGroup, vaultName, properties);

            var returnedVault = await Client.GetAsync(resGroup, vaultName);

            Assert.IsFalse(returnedVault.Value.Properties.EnableSoftDelete);

            await Client.DeleteAsync(resGroup, vaultName);
        }
    }
}
