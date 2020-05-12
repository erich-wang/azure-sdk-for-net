using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resource.Tests;
using Azure.Management.Resource.Models;
using System.IO;
using Azure.Management.Resource;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Azure.Core.Testing;
using System.Reflection;
using System.Threading;

namespace ResourceGroups.Tests
{
    public class LiveSubscriptionTests: ResourceOperationsTestsBase
    {
        public LiveSubscriptionTests(bool isAsync, ResourceManagementClientOptions.ServiceVersion serviceVersion)
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
                Initialize();
            }
        }

        [Test]
        public void ListSubscriptions()
        {
            var client = GetSubscriptionClient(context, handler);
            client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            var subscriptions = client.Subscriptions.List();

            Assert.NotNull(subscriptions);
            Assert.NotEmpty(subscriptions);
            Assert.NotNull(subscriptions.First().Id);
            Assert.NotNull(subscriptions.First().SubscriptionId);
            Assert.NotNull(subscriptions.First().DisplayName);
            Assert.NotNull(subscriptions.First().State);
        }

        [Test]
        public void GetSubscriptionDetails()
        {
            var client = GetSubscriptionClient(context, handler);
            var rmclient = GetResourceManagementClient(context, handler);
            client.SetRetryPolicy(new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(1));

            var subscriptionDetails = client.Subscriptions.Get(rmclient.SubscriptionId);

            Assert.NotNull(subscriptionDetails);
            Assert.NotNull(subscriptionDetails.Id);
            Assert.NotNull(subscriptionDetails.SubscriptionId);
            Assert.NotNull(subscriptionDetails.DisplayName);
            Assert.NotNull(subscriptionDetails.State);
            Assert.NotNull(subscriptionDetails.Tags);
        }
    }
}
