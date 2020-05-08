// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.EventHub.Models;

namespace Azure.Management.EventHub
{
    public partial class ConsumerGroupsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ConsumerGroupsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ConsumerGroupsClient for mocking. </summary>
        protected ConsumerGroupsClient()
        {
        }
        /// <summary> Initializes a new instance of ConsumerGroupsClient. </summary>
        internal ConsumerGroupsClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2017-04-01")
        {
            RestClient = new ConsumerGroupsRestClient(clientDiagnostics, pipeline, subscriptionId, host, apiVersion);
            _clientDiagnostics = clientDiagnostics;
            _pipeline = pipeline;
        }

        /// <summary> Creates or updates an Event Hubs consumer group as a nested resource within a Namespace. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="parameters"> Parameters supplied to create or update a consumer group resource. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ConsumerGroup>> CreateOrUpdateAsync(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, ConsumerGroup parameters, CancellationToken cancellationToken = default)
        {
            return await RestClient.CreateOrUpdateAsync(resourceGroupName, namespaceName, eventHubName, consumerGroupName, parameters, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Creates or updates an Event Hubs consumer group as a nested resource within a Namespace. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="parameters"> Parameters supplied to create or update a consumer group resource. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ConsumerGroup> CreateOrUpdate(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, ConsumerGroup parameters, CancellationToken cancellationToken = default)
        {
            return RestClient.CreateOrUpdate(resourceGroupName, namespaceName, eventHubName, consumerGroupName, parameters, cancellationToken);
        }

        /// <summary> Deletes a consumer group from the specified Event Hub and resource group. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response> DeleteAsync(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, CancellationToken cancellationToken = default)
        {
            return await RestClient.DeleteAsync(resourceGroupName, namespaceName, eventHubName, consumerGroupName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Deletes a consumer group from the specified Event Hub and resource group. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response Delete(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, CancellationToken cancellationToken = default)
        {
            return RestClient.Delete(resourceGroupName, namespaceName, eventHubName, consumerGroupName, cancellationToken);
        }

        /// <summary> Gets a description for the specified consumer group. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ConsumerGroup>> GetAsync(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, namespaceName, eventHubName, consumerGroupName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets a description for the specified consumer group. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="consumerGroupName"> The consumer group name. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ConsumerGroup> Get(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, namespaceName, eventHubName, consumerGroupName, cancellationToken);
        }

        /// <summary> Gets all the consumer groups in a Namespace. An empty feed is returned if no consumer group exists in the Namespace. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="skip"> Skip is only used if a previous operation returned a partial result. If a previous response contains a nextLink element, the value of the nextLink element will include a skip parameter that specifies a starting point to use for subsequent calls. </param>
        /// <param name="top"> May be used to limit the number of results to the most recent N usageDetails. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ConsumerGroup> ListByEventHubAsync(string resourceGroupName, string namespaceName, string eventHubName, int? skip = null, int? top = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (namespaceName == null)
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }
            if (eventHubName == null)
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            async Task<Page<ConsumerGroup>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListByEventHubAsync(resourceGroupName, namespaceName, eventHubName, skip, top, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ConsumerGroup>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListByEventHubNextPageAsync(nextLink, resourceGroupName, namespaceName, eventHubName, skip, top, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all the consumer groups in a Namespace. An empty feed is returned if no consumer group exists in the Namespace. </summary>
        /// <param name="resourceGroupName"> Name of the resource group within the azure subscription. </param>
        /// <param name="namespaceName"> The Namespace name. </param>
        /// <param name="eventHubName"> The Event Hub name. </param>
        /// <param name="skip"> Skip is only used if a previous operation returned a partial result. If a previous response contains a nextLink element, the value of the nextLink element will include a skip parameter that specifies a starting point to use for subsequent calls. </param>
        /// <param name="top"> May be used to limit the number of results to the most recent N usageDetails. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ConsumerGroup> ListByEventHub(string resourceGroupName, string namespaceName, string eventHubName, int? skip = null, int? top = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (namespaceName == null)
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }
            if (eventHubName == null)
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            Page<ConsumerGroup> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListByEventHub(resourceGroupName, namespaceName, eventHubName, skip, top, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ConsumerGroup> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListByEventHubNextPage(nextLink, resourceGroupName, namespaceName, eventHubName, skip, top, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }
    }
}
