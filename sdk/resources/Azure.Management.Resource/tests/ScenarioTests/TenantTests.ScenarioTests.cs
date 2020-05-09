using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Management.Resource;
using Azure.Management.Resource.Models;
using Azure.Management.Resource.Tests;
using NUnit.Framework;

namespace Azure.Management.Resource.Tests.ScenarioTests
{
    public class LiveTenantTests : ResourceOperationsTestsBase
    {
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
        public void ListTenants()
        {
            // NEXT environment variables used to record the mock

                var client = GetSubscriptionClient(context, handler);
                client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));
                var tenants = client.Tenants.ListWithHttpMessagesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.NotNull(tenants);
                Assert.Equal(HttpStatusCode.OK, tenants.Response.StatusCode);
                Assert.NotNull(tenants.Body);
                Assert.NotEmpty(tenants.Body);
                Assert.NotNull(tenants.Body.First().Id);
                Assert.NotNull(tenants.Body.First().TenantId);
        }
    }
}
