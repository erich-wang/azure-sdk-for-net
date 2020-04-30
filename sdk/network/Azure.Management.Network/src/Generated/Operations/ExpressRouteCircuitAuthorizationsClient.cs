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
    /// <summary> The ExpressRouteCircuitAuthorizations service client. </summary>
    public partial class ExpressRouteCircuitAuthorizationsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ExpressRouteCircuitAuthorizationsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ExpressRouteCircuitAuthorizationsClient for mocking. </summary>
        protected ExpressRouteCircuitAuthorizationsClient()
        {
        }

        /// <summary> Initializes a new instance of ExpressRouteCircuitAuthorizationsClient. </summary>
        public ExpressRouteCircuitAuthorizationsClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new ExpressRouteCircuitAuthorizationsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets the specified authorization from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ExpressRouteCircuitAuthorization>> GetAsync(string resourceGroupName, string circuitName, string authorizationName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, circuitName, authorizationName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets the specified authorization from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ExpressRouteCircuitAuthorization> Get(string resourceGroupName, string circuitName, string authorizationName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, circuitName, authorizationName, cancellationToken);
        }

        /// <summary> Gets all authorizations in an express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the circuit. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ExpressRouteCircuitAuthorization> ListAsync(string resourceGroupName, string circuitName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }

            async Task<Page<ExpressRouteCircuitAuthorization>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(resourceGroupName, circuitName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ExpressRouteCircuitAuthorization>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, resourceGroupName, circuitName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all authorizations in an express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the circuit. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ExpressRouteCircuitAuthorization> List(string resourceGroupName, string circuitName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }

            Page<ExpressRouteCircuitAuthorization> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(resourceGroupName, circuitName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ExpressRouteCircuitAuthorization> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, resourceGroupName, circuitName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Deletes the specified authorization from the specified express route circuit. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "ExpressRouteCircuitAuthorizationsClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes the specified authorization from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string circuitName, string authorizationName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (authorizationName == null)
            {
                throw new ArgumentNullException(nameof(authorizationName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, circuitName, authorizationName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, circuitName, authorizationName));
        }

        /// <summary> Deletes the specified authorization from the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string circuitName, string authorizationName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (authorizationName == null)
            {
                throw new ArgumentNullException(nameof(authorizationName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, circuitName, authorizationName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, circuitName, authorizationName));
        }

        /// <summary> Creates or updates an authorization in the specified express route circuit. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<ExpressRouteCircuitAuthorization> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "ExpressRouteCircuitAuthorizationsClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return ExpressRouteCircuitAuthorization.DeserializeExpressRouteCircuitAuthorization(document.RootElement);
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
                    return ExpressRouteCircuitAuthorization.DeserializeExpressRouteCircuitAuthorization(document.RootElement);
                }
            });
        }

        /// <summary> Creates or updates an authorization in the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="authorizationParameters"> Parameters supplied to the create or update express route circuit authorization operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<ExpressRouteCircuitAuthorization>> StartCreateOrUpdateAsync(string resourceGroupName, string circuitName, string authorizationName, ExpressRouteCircuitAuthorization authorizationParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (authorizationName == null)
            {
                throw new ArgumentNullException(nameof(authorizationName));
            }
            if (authorizationParameters == null)
            {
                throw new ArgumentNullException(nameof(authorizationParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, circuitName, authorizationName, authorizationParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, circuitName, authorizationName, authorizationParameters));
        }

        /// <summary> Creates or updates an authorization in the specified express route circuit. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="circuitName"> The name of the express route circuit. </param>
        /// <param name="authorizationName"> The name of the authorization. </param>
        /// <param name="authorizationParameters"> Parameters supplied to the create or update express route circuit authorization operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<ExpressRouteCircuitAuthorization> StartCreateOrUpdate(string resourceGroupName, string circuitName, string authorizationName, ExpressRouteCircuitAuthorization authorizationParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (circuitName == null)
            {
                throw new ArgumentNullException(nameof(circuitName));
            }
            if (authorizationName == null)
            {
                throw new ArgumentNullException(nameof(authorizationName));
            }
            if (authorizationParameters == null)
            {
                throw new ArgumentNullException(nameof(authorizationParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, circuitName, authorizationName, authorizationParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, circuitName, authorizationName, authorizationParameters));
        }
    }
}
