// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.Resource.Models;

namespace Azure.Management.Resource
{
    /// <summary> The Tags service client. </summary>
    public partial class TagsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal TagsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of TagsClient for mocking. </summary>
        protected TagsClient()
        {
        }

        /// <summary> Initializes a new instance of TagsClient. </summary>
        public TagsClient(string subscriptionId, TokenCredential tokenCredential, ResourceManagementClientOptions options = null)
        {
            options ??= new ResourceManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new TagsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId, apiVersion: options.Version);
        }

        /// <summary> Delete a subscription resource tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response> DeleteValueAsync(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            return await RestClient.DeleteValueAsync(tagName, tagValue, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Delete a subscription resource tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response DeleteValue(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            return RestClient.DeleteValue(tagName, tagValue, cancellationToken);
        }

        /// <summary> Create a subscription resource tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<TagValue>> CreateOrUpdateValueAsync(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            return await RestClient.CreateOrUpdateValueAsync(tagName, tagValue, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Create a subscription resource tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<TagValue> CreateOrUpdateValue(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            return RestClient.CreateOrUpdateValue(tagName, tagValue, cancellationToken);
        }

        /// <summary> Create a subscription resource tag. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<TagDetails>> CreateOrUpdateAsync(string tagName, CancellationToken cancellationToken = default)
        {
            return await RestClient.CreateOrUpdateAsync(tagName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Create a subscription resource tag. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<TagDetails> CreateOrUpdate(string tagName, CancellationToken cancellationToken = default)
        {
            return RestClient.CreateOrUpdate(tagName, cancellationToken);
        }

        /// <summary> Delete a subscription resource tag. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response> DeleteAsync(string tagName, CancellationToken cancellationToken = default)
        {
            return await RestClient.DeleteAsync(tagName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Delete a subscription resource tag. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response Delete(string tagName, CancellationToken cancellationToken = default)
        {
            return RestClient.Delete(tagName, cancellationToken);
        }

        /// <summary> Get a list of subscription resource tags. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<TagDetails> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<TagDetails>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<TagDetails>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Get a list of subscription resource tags. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<TagDetails> List(CancellationToken cancellationToken = default)
        {
            Page<TagDetails> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<TagDetails> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }
    }
}
