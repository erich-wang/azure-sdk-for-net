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
    /// <summary> The P2SVpnGateways service client. </summary>
    public partial class P2SVpnGatewaysClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal P2SVpnGatewaysRestClient RestClient { get; }
        /// <summary> Initializes a new instance of P2SVpnGatewaysClient for mocking. </summary>
        protected P2SVpnGatewaysClient()
        {
        }

        /// <summary> Initializes a new instance of P2SVpnGatewaysClient. </summary>
        public P2SVpnGatewaysClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new P2SVpnGatewaysRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Retrieves the details of a virtual wan p2s vpn gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<P2SVpnGateway>> GetAsync(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, gatewayName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Retrieves the details of a virtual wan p2s vpn gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<P2SVpnGateway> Get(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, gatewayName, cancellationToken);
        }

        /// <summary> Updates virtual wan p2s vpn gateway tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<P2SVpnGateway>> UpdateTagsAsync(string resourceGroupName, string gatewayName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return await RestClient.UpdateTagsAsync(resourceGroupName, gatewayName, tags, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Updates virtual wan p2s vpn gateway tags. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<P2SVpnGateway> UpdateTags(string resourceGroupName, string gatewayName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return RestClient.UpdateTags(resourceGroupName, gatewayName, tags, cancellationToken);
        }

        /// <summary> Lists all the P2SVpnGateways in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<P2SVpnGateway> ListByResourceGroupAsync(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            async Task<Page<P2SVpnGateway>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupAsync(resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<P2SVpnGateway>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupNextPageAsync(nextLink, resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the P2SVpnGateways in a resource group. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<P2SVpnGateway> ListByResourceGroup(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            Page<P2SVpnGateway> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroup(resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<P2SVpnGateway> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroupNextPage(nextLink, resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the P2SVpnGateways in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<P2SVpnGateway> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<P2SVpnGateway>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<P2SVpnGateway>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all the P2SVpnGateways in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<P2SVpnGateway> List(CancellationToken cancellationToken = default)
        {
            Page<P2SVpnGateway> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<P2SVpnGateway> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Creates a virtual wan p2s vpn gateway if it doesn&apos;t exist else updates the existing gateway. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<P2SVpnGateway> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "P2SVpnGatewaysClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return P2SVpnGateway.DeserializeP2SVpnGateway(document.RootElement);
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
                    return P2SVpnGateway.DeserializeP2SVpnGateway(document.RootElement);
                }
            });
        }

        /// <summary> Creates a virtual wan p2s vpn gateway if it doesn&apos;t exist else updates the existing gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="p2SVpnGatewayParameters"> Parameters supplied to create or Update a virtual wan p2s vpn gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<P2SVpnGateway>> StartCreateOrUpdateAsync(string resourceGroupName, string gatewayName, P2SVpnGateway p2SVpnGatewayParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }
            if (p2SVpnGatewayParameters == null)
            {
                throw new ArgumentNullException(nameof(p2SVpnGatewayParameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, gatewayName, p2SVpnGatewayParameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, gatewayName, p2SVpnGatewayParameters));
        }

        /// <summary> Creates a virtual wan p2s vpn gateway if it doesn&apos;t exist else updates the existing gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="p2SVpnGatewayParameters"> Parameters supplied to create or Update a virtual wan p2s vpn gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<P2SVpnGateway> StartCreateOrUpdate(string resourceGroupName, string gatewayName, P2SVpnGateway p2SVpnGatewayParameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }
            if (p2SVpnGatewayParameters == null)
            {
                throw new ArgumentNullException(nameof(p2SVpnGatewayParameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, gatewayName, p2SVpnGatewayParameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, gatewayName, p2SVpnGatewayParameters));
        }

        /// <summary> Deletes a virtual wan p2s vpn gateway. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "P2SVpnGatewaysClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes a virtual wan p2s vpn gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, gatewayName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, gatewayName));
        }

        /// <summary> Deletes a virtual wan p2s vpn gateway. </summary>
        /// <param name="resourceGroupName"> The resource group name of the P2SVpnGateway. </param>
        /// <param name="gatewayName"> The name of the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, gatewayName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, gatewayName));
        }

        /// <summary> Generates VPN profile for P2S client of the P2SVpnGateway in the specified resource group. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<VpnProfileResponse> CreateGenerateVpnProfile(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Post, "P2SVpnGatewaysClient.GenerateVpnProfile", OperationFinalStateVia.Location, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return VpnProfileResponse.DeserializeVpnProfileResponse(document.RootElement);
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
                    return VpnProfileResponse.DeserializeVpnProfileResponse(document.RootElement);
                }
            });
        }

        /// <summary> Generates VPN profile for P2S client of the P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="authenticationMethod"> VPN client authentication method. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<VpnProfileResponse>> StartGenerateVpnProfileAsync(string resourceGroupName, string gatewayName, AuthenticationMethod? authenticationMethod = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = await RestClient.GenerateVpnProfileAsync(resourceGroupName, gatewayName, authenticationMethod, cancellationToken).ConfigureAwait(false);
            return CreateGenerateVpnProfile(originalResponse, () => RestClient.CreateGenerateVpnProfileRequest(resourceGroupName, gatewayName, authenticationMethod));
        }

        /// <summary> Generates VPN profile for P2S client of the P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="authenticationMethod"> VPN client authentication method. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<VpnProfileResponse> StartGenerateVpnProfile(string resourceGroupName, string gatewayName, AuthenticationMethod? authenticationMethod = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = RestClient.GenerateVpnProfile(resourceGroupName, gatewayName, authenticationMethod, cancellationToken);
            return CreateGenerateVpnProfile(originalResponse, () => RestClient.CreateGenerateVpnProfileRequest(resourceGroupName, gatewayName, authenticationMethod));
        }

        /// <summary> Gets the connection health of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<P2SVpnGateway> CreateGetP2SVpnConnectionHealth(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Post, "P2SVpnGatewaysClient.GetP2SVpnConnectionHealth", OperationFinalStateVia.Location, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return P2SVpnGateway.DeserializeP2SVpnGateway(document.RootElement);
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
                    return P2SVpnGateway.DeserializeP2SVpnGateway(document.RootElement);
                }
            });
        }

        /// <summary> Gets the connection health of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<P2SVpnGateway>> StartGetP2SVpnConnectionHealthAsync(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = await RestClient.GetP2SVpnConnectionHealthAsync(resourceGroupName, gatewayName, cancellationToken).ConfigureAwait(false);
            return CreateGetP2SVpnConnectionHealth(originalResponse, () => RestClient.CreateGetP2SVpnConnectionHealthRequest(resourceGroupName, gatewayName));
        }

        /// <summary> Gets the connection health of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<P2SVpnGateway> StartGetP2SVpnConnectionHealth(string resourceGroupName, string gatewayName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = RestClient.GetP2SVpnConnectionHealth(resourceGroupName, gatewayName, cancellationToken);
            return CreateGetP2SVpnConnectionHealth(originalResponse, () => RestClient.CreateGetP2SVpnConnectionHealthRequest(resourceGroupName, gatewayName));
        }

        /// <summary> Gets the sas url to get the connection health detail of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<P2SVpnConnectionHealth> CreateGetP2SVpnConnectionHealthDetailed(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Post, "P2SVpnGatewaysClient.GetP2SVpnConnectionHealthDetailed", OperationFinalStateVia.Location, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return P2SVpnConnectionHealth.DeserializeP2SVpnConnectionHealth(document.RootElement);
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
                    return P2SVpnConnectionHealth.DeserializeP2SVpnConnectionHealth(document.RootElement);
                }
            });
        }

        /// <summary> Gets the sas url to get the connection health detail of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="vpnUserNamesFilter"> The list of p2s vpn user names whose p2s vpn connection detailed health to retrieve for. </param>
        /// <param name="outputBlobSasUrl"> The sas-url to download the P2S Vpn connection health detail. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<P2SVpnConnectionHealth>> StartGetP2SVpnConnectionHealthDetailedAsync(string resourceGroupName, string gatewayName, IEnumerable<string> vpnUserNamesFilter = null, string outputBlobSasUrl = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = await RestClient.GetP2SVpnConnectionHealthDetailedAsync(resourceGroupName, gatewayName, vpnUserNamesFilter, outputBlobSasUrl, cancellationToken).ConfigureAwait(false);
            return CreateGetP2SVpnConnectionHealthDetailed(originalResponse, () => RestClient.CreateGetP2SVpnConnectionHealthDetailedRequest(resourceGroupName, gatewayName, vpnUserNamesFilter, outputBlobSasUrl));
        }

        /// <summary> Gets the sas url to get the connection health detail of P2S clients of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="gatewayName"> The name of the P2SVpnGateway. </param>
        /// <param name="vpnUserNamesFilter"> The list of p2s vpn user names whose p2s vpn connection detailed health to retrieve for. </param>
        /// <param name="outputBlobSasUrl"> The sas-url to download the P2S Vpn connection health detail. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<P2SVpnConnectionHealth> StartGetP2SVpnConnectionHealthDetailed(string resourceGroupName, string gatewayName, IEnumerable<string> vpnUserNamesFilter = null, string outputBlobSasUrl = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (gatewayName == null)
            {
                throw new ArgumentNullException(nameof(gatewayName));
            }

            var originalResponse = RestClient.GetP2SVpnConnectionHealthDetailed(resourceGroupName, gatewayName, vpnUserNamesFilter, outputBlobSasUrl, cancellationToken);
            return CreateGetP2SVpnConnectionHealthDetailed(originalResponse, () => RestClient.CreateGetP2SVpnConnectionHealthDetailedRequest(resourceGroupName, gatewayName, vpnUserNamesFilter, outputBlobSasUrl));
        }

        /// <summary> Disconnect P2S vpn connections of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<Response> CreateDisconnectP2SVpnConnections(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Post, "P2SVpnGatewaysClient.DisconnectP2SVpnConnections", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Disconnect P2S vpn connections of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="p2SVpnGatewayName"> The name of the P2S Vpn Gateway. </param>
        /// <param name="vpnConnectionIds"> List of p2s vpn connection Ids. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDisconnectP2SVpnConnectionsAsync(string resourceGroupName, string p2SVpnGatewayName, IEnumerable<string> vpnConnectionIds = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (p2SVpnGatewayName == null)
            {
                throw new ArgumentNullException(nameof(p2SVpnGatewayName));
            }

            var originalResponse = await RestClient.DisconnectP2SVpnConnectionsAsync(resourceGroupName, p2SVpnGatewayName, vpnConnectionIds, cancellationToken).ConfigureAwait(false);
            return CreateDisconnectP2SVpnConnections(originalResponse, () => RestClient.CreateDisconnectP2SVpnConnectionsRequest(resourceGroupName, p2SVpnGatewayName, vpnConnectionIds));
        }

        /// <summary> Disconnect P2S vpn connections of the virtual wan P2SVpnGateway in the specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="p2SVpnGatewayName"> The name of the P2S Vpn Gateway. </param>
        /// <param name="vpnConnectionIds"> List of p2s vpn connection Ids. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDisconnectP2SVpnConnections(string resourceGroupName, string p2SVpnGatewayName, IEnumerable<string> vpnConnectionIds = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (p2SVpnGatewayName == null)
            {
                throw new ArgumentNullException(nameof(p2SVpnGatewayName));
            }

            var originalResponse = RestClient.DisconnectP2SVpnConnections(resourceGroupName, p2SVpnGatewayName, vpnConnectionIds, cancellationToken);
            return CreateDisconnectP2SVpnConnections(originalResponse, () => RestClient.CreateDisconnectP2SVpnConnectionsRequest(resourceGroupName, p2SVpnGatewayName, vpnConnectionIds));
        }
    }
}
