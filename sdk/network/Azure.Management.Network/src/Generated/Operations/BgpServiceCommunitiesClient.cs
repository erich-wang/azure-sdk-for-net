// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.Network.Models;

namespace Azure.Management.Network
{
    /// <summary> The BgpServiceCommunities service client. </summary>
    public partial class BgpServiceCommunitiesClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal BgpServiceCommunitiesRestClient RestClient { get; }
        /// <summary> Initializes a new instance of BgpServiceCommunitiesClient for mocking. </summary>
        protected BgpServiceCommunitiesClient()
        {
        }

        /// <summary> Initializes a new instance of BgpServiceCommunitiesClient. </summary>
        public BgpServiceCommunitiesClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new BgpServiceCommunitiesRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets all the available bgp service communities. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<BgpServiceCommunity> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<BgpServiceCommunity>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<BgpServiceCommunity>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all the available bgp service communities. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<BgpServiceCommunity> List(CancellationToken cancellationToken = default)
        {
            Page<BgpServiceCommunity> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<BgpServiceCommunity> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }
    }
}
