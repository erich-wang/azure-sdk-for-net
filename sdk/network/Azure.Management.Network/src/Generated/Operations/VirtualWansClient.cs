// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.Network.Models;

namespace Azure.Management.Network
{
    /// <summary> The VirtualWans service client. </summary>
    public partial class VirtualWansClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal VirtualWansRestClient RestClient { get; }
        /// <summary> Initializes a new instance of VirtualWansClient for mocking. </summary>
        protected VirtualWansClient()
        {
        }

        /// <summary> Initializes a new instance of VirtualWansClient. </summary>
        public VirtualWansClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new VirtualWansRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Retrieves the details of a VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being retrieved. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<VirtualWAN>> GetAsync(string resourceGroupName, string virtualWANName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, virtualWANName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Retrieves the details of a VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being retrieved. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<VirtualWAN> Get(string resourceGroupName, string virtualWANName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, virtualWANName, cancellationToken);
        }

        /// <summary> Updates a VirtualWAN tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being updated. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<VirtualWAN>> UpdateTagsAsync(string resourceGroupName, string virtualWANName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return await RestClient.UpdateTagsAsync(resourceGroupName, virtualWANName, tags, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Updates a VirtualWAN tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being updated. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<VirtualWAN> UpdateTags(string resourceGroupName, string virtualWANName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return RestClient.UpdateTags(resourceGroupName, virtualWANName, tags, cancellationToken);
        }

        /// <summary> Lists all the VirtualWANs in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<VirtualWAN> ListByResourceGroupAsync(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            async Task<Page<VirtualWAN>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupAsync(resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<VirtualWAN>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupNextPageAsync(nextLink, resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the VirtualWANs in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<VirtualWAN> ListByResourceGroup(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            Page<VirtualWAN> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroup(resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<VirtualWAN> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroupNextPage(nextLink, resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the VirtualWANs in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<VirtualWAN> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<VirtualWAN>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<VirtualWAN>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the VirtualWANs in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<VirtualWAN> List(CancellationToken cancellationToken = default)
        {
            Page<VirtualWAN> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<VirtualWAN> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Creates a VirtualWAN resource if it doesn&apos;t exist else updates the existing VirtualWAN. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<VirtualWAN> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "VirtualWansClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return VirtualWAN.DeserializeVirtualWAN(document.RootElement);
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
                    return VirtualWAN.DeserializeVirtualWAN(document.RootElement);
                }
            });
        }

        /// <summary> Creates a VirtualWAN resource if it doesn&apos;t exist else updates the existing VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being created or updated. </param>
        /// <param name="wANParameters"> Parameters supplied to create or update VirtualWAN. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<VirtualWAN>> StartCreateOrUpdateAsync(string resourceGroupName, string virtualWANName, VirtualWAN wANParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (virtualWANName == null)
            {
                throw new ArgumentNullException(nameof(virtualWANName));
            }
            if (wANParameters == null)
            {
                throw new ArgumentNullException(nameof(wANParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, virtualWANName, wANParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, virtualWANName, wANParameters));
        }

        /// <summary> Creates a VirtualWAN resource if it doesn&apos;t exist else updates the existing VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being created or updated. </param>
        /// <param name="wANParameters"> Parameters supplied to create or update VirtualWAN. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<VirtualWAN> StartCreateOrUpdate(string resourceGroupName, string virtualWANName, VirtualWAN wANParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (virtualWANName == null)
            {
                throw new ArgumentNullException(nameof(virtualWANName));
            }
            if (wANParameters == null)
            {
                throw new ArgumentNullException(nameof(wANParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, virtualWANName, wANParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, virtualWANName, wANParameters));
        }

        /// <summary> Deletes a VirtualWAN. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "VirtualWansClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes a VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being deleted. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string virtualWANName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (virtualWANName == null)
            {
                throw new ArgumentNullException(nameof(virtualWANName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, virtualWANName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, virtualWANName));
        }

        /// <summary> Deletes a VirtualWAN. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VirtualWan. </param>
        /// <param name="virtualWANName"> The name of the VirtualWAN being deleted. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string virtualWANName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (virtualWANName == null)
            {
                throw new ArgumentNullException(nameof(virtualWANName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, virtualWANName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, virtualWANName));
        }
    }
}
