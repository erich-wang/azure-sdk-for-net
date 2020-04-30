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
    /// <summary> The ExpressRouteCircuitPeerings service client. </summary>
    public partial class ExpressRouteCircuitPeeringsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ExpressRouteCircuitPeeringsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ExpressRouteCircuitPeeringsClient for mocking. </summary>
        protected ExpressRouteCircuitPeeringsClient()
        {
        }

        /// <summary> Initializes a new instance of ExpressRouteCircuitPeeringsClient. </summary>
        public ExpressRouteCircuitPeeringsClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new ExpressRouteCircuitPeeringsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets the specified peering for the express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ExpressRouteCircuitPeering>> GetAsync(string resourceGroupName, string circuitName, string peeringName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, circuitName, peeringName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets the specified peering for the express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ExpressRouteCircuitPeering> Get(string resourceGroupName, string circuitName, string peeringName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, circuitName, peeringName, cancellationToken);
        }

        /// <summary> Gets all peerings in a specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ExpressRouteCircuitPeering> ListAsync(string resourceGroupName, string circuitName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }

            async Task<Page<ExpressRouteCircuitPeering>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(resourceGroupName, circuitName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ExpressRouteCircuitPeering>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, resourceGroupName, circuitName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all peerings in a specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ExpressRouteCircuitPeering> List(string resourceGroupName, string circuitName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }

            Page<ExpressRouteCircuitPeering> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(resourceGroupName, circuitName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ExpressRouteCircuitPeering> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, resourceGroupName, circuitName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Deletes the specified peering from the specified express route circuit. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "ExpressRouteCircuitPeeringsClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes the specified peering from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string circuitName, string peeringName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, circuitName, peeringName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, circuitName, peeringName));
        }

        /// <summary> Deletes the specified peering from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string circuitName, string peeringName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, circuitName, peeringName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, circuitName, peeringName));
        }

        /// <summary> Creates or updates a peering in the specified express route circuits. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<ExpressRouteCircuitPeering> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "ExpressRouteCircuitPeeringsClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return ExpressRouteCircuitPeering.DeserializeExpressRouteCircuitPeering(document.RootElement);
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
                    return ExpressRouteCircuitPeering.DeserializeExpressRouteCircuitPeering(document.RootElement);
                }
            });
        }

        /// <summary> Creates or updates a peering in the specified express route circuits. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="peeringParameters"> Parameters supplied to the create or update express route circuit peering operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<ExpressRouteCircuitPeering>> StartCreateOrUpdateAsync(string resourceGroupName, string circuitName, string peeringName, ExpressRouteCircuitPeering peeringParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }
            if (peeringParameters == null)
            {
                throw new ArgumentNullException(nameof(peeringParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, circuitName, peeringName, peeringParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, circuitName, peeringName, peeringParameters));
        }

        /// <summary> Creates or updates a peering in the specified express route circuits. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="peeringName"> The name of the peering. </param>
        /// <param name="peeringParameters"> Parameters supplied to the create or update express route circuit peering operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<ExpressRouteCircuitPeering> StartCreateOrUpdate(string resourceGroupName, string circuitName, string peeringName, ExpressRouteCircuitPeering peeringParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (peeringName == null)
            {
                throw new ArgumentNullException(nameof(peeringName));
            }
            if (peeringParameters == null)
            {
                throw new ArgumentNullException(nameof(peeringParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, circuitName, peeringName, peeringParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, circuitName, peeringName, peeringParameters));
        }
    }
}
