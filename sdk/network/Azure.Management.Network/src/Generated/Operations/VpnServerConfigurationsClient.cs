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
    /// <summary> The VpnServerConfigurations service client. </summary>
    public partial class VpnServerConfigurationsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal VpnServerConfigurationsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of VpnServerConfigurationsClient for mocking. </summary>
        protected VpnServerConfigurationsClient()
        {
        }

        /// <summary> Initializes a new instance of VpnServerConfigurationsClient. </summary>
        public VpnServerConfigurationsClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new VpnServerConfigurationsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Retrieves the details of a VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being retrieved. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<VpnServerConfiguration>> GetAsync(string resourceGroupName, string vpnServerConfigurationName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, vpnServerConfigurationName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Retrieves the details of a VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being retrieved. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<VpnServerConfiguration> Get(string resourceGroupName, string vpnServerConfigurationName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, vpnServerConfigurationName, cancellationToken);
        }

        /// <summary> Updates VpnServerConfiguration tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being updated. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<VpnServerConfiguration>> UpdateTagsAsync(string resourceGroupName, string vpnServerConfigurationName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return await RestClient.UpdateTagsAsync(resourceGroupName, vpnServerConfigurationName, tags, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Updates VpnServerConfiguration tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being updated. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<VpnServerConfiguration> UpdateTags(string resourceGroupName, string vpnServerConfigurationName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return RestClient.UpdateTags(resourceGroupName, vpnServerConfigurationName, tags, cancellationToken);
        }

        /// <summary> Lists all the vpnServerConfigurations in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<VpnServerConfiguration> ListByResourceGroupAsync(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            async Task<Page<VpnServerConfiguration>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupAsync(resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<VpnServerConfiguration>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupNextPageAsync(nextLink, resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the vpnServerConfigurations in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<VpnServerConfiguration> ListByResourceGroup(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            Page<VpnServerConfiguration> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroup(resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<VpnServerConfiguration> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroupNextPage(nextLink, resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the VpnServerConfigurations in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<VpnServerConfiguration> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<VpnServerConfiguration>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<VpnServerConfiguration>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the VpnServerConfigurations in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<VpnServerConfiguration> List(CancellationToken cancellationToken = default)
        {
            Page<VpnServerConfiguration> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<VpnServerConfiguration> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Creates a VpnServerConfiguration resource if it doesn&apos;t exist else updates the existing VpnServerConfiguration. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<VpnServerConfiguration> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "VpnServerConfigurationsClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return VpnServerConfiguration.DeserializeVpnServerConfiguration(document.RootElement);
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
                    return VpnServerConfiguration.DeserializeVpnServerConfiguration(document.RootElement);
                }
            });
        }

        /// <summary> Creates a VpnServerConfiguration resource if it doesn&apos;t exist else updates the existing VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being created or updated. </param>
        /// <param name="vpnServerConfigurationParameters"> Parameters supplied to create or update VpnServerConfiguration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<VpnServerConfiguration>> StartCreateOrUpdateAsync(string resourceGroupName, string vpnServerConfigurationName, VpnServerConfiguration vpnServerConfigurationParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (vpnServerConfigurationName == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationName));
            }
            if (vpnServerConfigurationParameters == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, vpnServerConfigurationName, vpnServerConfigurationParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, vpnServerConfigurationName, vpnServerConfigurationParameters));
        }

        /// <summary> Creates a VpnServerConfiguration resource if it doesn&apos;t exist else updates the existing VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being created or updated. </param>
        /// <param name="vpnServerConfigurationParameters"> Parameters supplied to create or update VpnServerConfiguration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<VpnServerConfiguration> StartCreateOrUpdate(string resourceGroupName, string vpnServerConfigurationName, VpnServerConfiguration vpnServerConfigurationParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (vpnServerConfigurationName == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationName));
            }
            if (vpnServerConfigurationParameters == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, vpnServerConfigurationName, vpnServerConfigurationParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, vpnServerConfigurationName, vpnServerConfigurationParameters));
        }

        /// <summary> Deletes a VpnServerConfiguration. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "VpnServerConfigurationsClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes a VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being deleted. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string vpnServerConfigurationName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (vpnServerConfigurationName == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, vpnServerConfigurationName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, vpnServerConfigurationName));
        }

        /// <summary> Deletes a VpnServerConfiguration. </summary>
        /// <param name="resourceGroupName"> The resource group name of the VpnServerConfiguration. </param>
        /// <param name="vpnServerConfigurationName"> The name of the VpnServerConfiguration being deleted. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string vpnServerConfigurationName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (vpnServerConfigurationName == null)
            {
                throw new ArgumentNullException(nameof(vpnServerConfigurationName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, vpnServerConfigurationName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, vpnServerConfigurationName));
        }
    }
}
