// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.Network.Models;

namespace Azure.Management.Network
{
    /// <summary> The ExpressRouteCrossConnectionPeerings service client. </summary>
    public partial class ExpressRouteCrossConnectionPeeringsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ExpressRouteCrossConnectionPeeringsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ExpressRouteCrossConnectionPeeringsClient for mocking. </summary>
        protected ExpressRouteCrossConnectionPeeringsClient()
        {
        }

        /// <summary> Initializes a new instance of ExpressRouteCrossConnectionPeeringsClient. </summary>
        public ExpressRouteCrossConnectionPeeringsClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new ExpressRouteCrossConnectionPeeringsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets the specified peering for the ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ExpressRouteCrossConnectionPeering>> GetAsync(string resourceGroupName, string crossConnectionName, string peeringName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, crossConnectionName, peeringName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets the specified peering for the ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ExpressRouteCrossConnectionPeering> Get(string resourceGroupName, string crossConnectionName, string peeringName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, crossConnectionName, peeringName, cancellationToken);
        }

        /// <summary> Gets all peerings in a specified ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ExpressRouteCrossConnectionPeering> ListAsync(string resourceGroupName, string crossConnectionName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }

            async Task<Page<ExpressRouteCrossConnectionPeering>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(resourceGroupName, crossConnectionName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ExpressRouteCrossConnectionPeering>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, resourceGroupName, crossConnectionName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all peerings in a specified ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ExpressRouteCrossConnectionPeering> List(string resourceGroupName, string crossConnectionName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }

            Page<ExpressRouteCrossConnectionPeering> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(resourceGroupName, crossConnectionName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ExpressRouteCrossConnectionPeering> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, resourceGroupName, crossConnectionName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Deletes the specified peering from the ExpressRouteCrossConnection. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<Response> CreateDelete(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "ExpressRouteCrossConnectionPeeringsClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes the specified peering from the ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string crossConnectionName, string peeringName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, crossConnectionName, peeringName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, crossConnectionName, peeringName));
        }

        /// <summary> Deletes the specified peering from the ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string crossConnectionName, string peeringName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, crossConnectionName, peeringName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, crossConnectionName, peeringName));
        }

        /// <summary> Creates or updates a peering in the specified ExpressRouteCrossConnection. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<ExpressRouteCrossConnectionPeering> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "ExpressRouteCrossConnectionPeeringsClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return ExpressRouteCrossConnectionPeering.DeserializeExpressRouteCrossConnectionPeering(document.RootElement);
                }
            },
            async (response, cancellationToken) =>
            {
                using var document = await JsonDocument.ParseAsync(response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return ExpressRouteCrossConnectionPeering.DeserializeExpressRouteCrossConnectionPeering(document.RootElement);
                }
            });
        }

        /// <summary> Creates or updates a peering in the specified ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="peeringParameters"> Parameters supplied to the create or update ExpressRouteCrossConnection peering operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<ExpressRouteCrossConnectionPeering>> StartCreateOrUpdateAsync(string resourceGroupName, string crossConnectionName, string peeringName, ExpressRouteCrossConnectionPeering peeringParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }
            if (peeringParameters == null)
            {
                throw new ArgumentNullException(nameof(peeringParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, crossConnectionName, peeringName, peeringParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, crossConnectionName, peeringName, peeringParameters));
        }

        /// <summary> Creates or updates a peering in the specified ExpressRouteCrossConnection. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="crossConnectionName"> The name of the ExpressRouteCrossConnection. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="peeringParameters"> Parameters supplied to the create or update ExpressRouteCrossConnection peering operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<ExpressRouteCrossConnectionPeering> StartCreateOrUpdate(string resourceGroupName, string crossConnectionName, string peeringName, ExpressRouteCrossConnectionPeering peeringParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (crossConnectionName == null)
            {
                throw new ArgumentNullException(nameof(crossConnectionName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }
            if (peeringParameters == null)
            {
                throw new ArgumentNullException(nameof(peeringParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, crossConnectionName, peeringName, peeringParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, crossConnectionName, peeringName, peeringParameters));
        }
    }
}
