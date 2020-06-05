// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Tests;
using NUnit.Framework;

namespace ResourceGroups.Tests
{
    public class LiveTenantTests : ResourceOperationsTestsBase
    {
        public LiveTenantTests(bool isAsync)
            : base(isAsync)
        {
        }


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
        public async Task ListTenants()
        {
            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            var tenants = await TenantsClient.ListAsync().ToEnumerableAsync();

            Assert.NotNull(tenants);
            //Assert.Equal(HttpStatusCode.OK, tenants.Response.StatusCode);
            //Assert.NotNull(tenants.Body);
            //Assert.NotEmpty(tenants.Body);
            Assert.NotNull(tenants.First().Id);
            Assert.NotNull(tenants.First().TenantId);
        }
    }
}
