﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Tests;
using NUnit.Framework;

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
