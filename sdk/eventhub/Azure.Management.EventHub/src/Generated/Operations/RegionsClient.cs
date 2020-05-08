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
    public partial class RegionsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal RegionsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of RegionsClient for mocking. </summary>
        protected RegionsClient()
        {
        }
        /// <summary> Initializes a new instance of RegionsClient. </summary>
        internal RegionsClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2017-04-01")
        {
            RestClient = new RegionsRestClient(clientDiagnostics, pipeline, subscriptionId, host, apiVersion);
            _clientDiagnostics = clientDiagnostics;
            _pipeline = pipeline;
        }

        /// <summary> Gets the available Regions for a given sku. </summary>
        /// <param name="sku"> The sku type. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<MessagingRegions> ListBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            if (sku == null)
            {
                throw new ArgumentNullException(nameof(sku));
            }

            async Task<Page<MessagingRegions>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListBySkuAsync(sku, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<MessagingRegions>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListBySkuNextPageAsync(nextLink, sku, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets the available Regions for a given sku. </summary>
        /// <param name="sku"> The sku type. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<MessagingRegions> ListBySku(string sku, CancellationToken cancellationToken = default)
        {
            if (sku == null)
            {
                throw new ArgumentNullException(nameof(sku));
            }

            Page<MessagingRegions> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListBySku(sku, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<MessagingRegions> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListBySkuNextPage(nextLink, sku, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }
    }
}
