using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources.Tests;
using Azure.Management.Resources.Models;
using System.IO;
using Azure.Management.Resources;
using NUnit.Framework;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using System.Reflection;

namespace ResourceGroups.Tests
{
    public class LiveSubscriptionTests : ResourceOperationsTestsBase
    {
        public LiveSubscriptionTests(bool isAsync)
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
        public async Task ListSubscriptions()
        {
            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            var subscriptions = await SubscriptionsClient.ListAsync().ToEnumerableAsync();

            Assert.NotNull(subscriptions);
            Assert.IsNotEmpty(subscriptions);
            Assert.NotNull(subscriptions.First().Id);
            Assert.NotNull(subscriptions.First().SubscriptionId);
            Assert.NotNull(subscriptions.First().DisplayName);
            Assert.NotNull(subscriptions.First().State);
        }

        [Test]
        public async Task GetSubscriptionDetails()
        {
            //client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            var subscriptionDetails = (await SubscriptionsClient.GetAsync(TestEnvironment.SubscriptionId)).Value;

            Assert.NotNull(subscriptionDetails);
            Assert.NotNull(subscriptionDetails.Id);
            Assert.NotNull(subscriptionDetails.SubscriptionId);
            Assert.NotNull(subscriptionDetails.DisplayName);
            Assert.NotNull(subscriptionDetails.State);
            Assert.NotNull(subscriptionDetails.Tags);
        }
    }
}
