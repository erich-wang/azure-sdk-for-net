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
    /// <summary> The ServiceEndpointPolicies service client. </summary>
    public partial class ServiceEndpointPoliciesClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ServiceEndpointPoliciesRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ServiceEndpointPoliciesClient for mocking. </summary>
        protected ServiceEndpointPoliciesClient()
        {
        }

        /// <summary> Initializes a new instance of ServiceEndpointPoliciesClient. </summary>
        public ServiceEndpointPoliciesClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new ServiceEndpointPoliciesRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets the specified service Endpoint Policies in a specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="expand"> Expands referenced resources. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ServiceEndpointPolicy>> GetAsync(string resourceGroupName, string serviceEndpointPolicyName, string expand = null, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, serviceEndpointPolicyName, expand, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets the specified service Endpoint Policies in a specified resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="expand"> Expands referenced resources. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ServiceEndpointPolicy> Get(string resourceGroupName, string serviceEndpointPolicyName, string expand = null, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, serviceEndpointPolicyName, expand, cancellationToken);
        }

        /// <summary> Updates tags of a service endpoint policy. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ServiceEndpointPolicy>> UpdateTagsAsync(string resourceGroupName, string serviceEndpointPolicyName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return await RestClient.UpdateTagsAsync(resourceGroupName, serviceEndpointPolicyName, tags, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Updates tags of a service endpoint policy. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="tags"> Resource tags. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ServiceEndpointPolicy> UpdateTags(string resourceGroupName, string serviceEndpointPolicyName, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default)
        {
            return RestClient.UpdateTags(resourceGroupName, serviceEndpointPolicyName, tags, cancellationToken);
        }

        /// <summary> Gets all the service endpoint policies in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ServiceEndpointPolicy> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<ServiceEndpointPolicy>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ServiceEndpointPolicy>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all the service endpoint policies in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ServiceEndpointPolicy> List(CancellationToken cancellationToken = default)
        {
            Page<ServiceEndpointPolicy> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ServiceEndpointPolicy> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all service endpoint Policies in a resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<ServiceEndpointPolicy> ListByResourceGroupAsync(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            async Task<Page<ServiceEndpointPolicy>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupAsync(resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<ServiceEndpointPolicy>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListByResourceGroupNextPageAsync(nextLink, resourceGroupName, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Gets all service endpoint Policies in a resource group. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<ServiceEndpointPolicy> ListByResourceGroup(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }

            Page<ServiceEndpointPolicy> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroup(resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<ServiceEndpointPolicy> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListByResourceGroupNextPage(nextLink, resourceGroupName, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Deletes the specified service endpoint policy. </summary>
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

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Delete, "ServiceEndpointPoliciesClient.Delete", OperationFinalStateVia.Location, createOriginalHttpMessage);
        }

        /// <summary> Deletes the specified service endpoint policy. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<Response>> StartDeleteAsync(string resourceGroupName, string serviceEndpointPolicyName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (serviceEndpointPolicyName == null)
            {
                throw new ArgumentNullException(nameof(serviceEndpointPolicyName));
            }

            var originalResponse = await RestClient.DeleteAsync(resourceGroupName, serviceEndpointPolicyName, cancellationToken).ConfigureAwait(false);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, serviceEndpointPolicyName));
        }

        /// <summary> Deletes the specified service endpoint policy. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<Response> StartDelete(string resourceGroupName, string serviceEndpointPolicyName, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (serviceEndpointPolicyName == null)
            {
                throw new ArgumentNullException(nameof(serviceEndpointPolicyName));
            }

            var originalResponse = RestClient.Delete(resourceGroupName, serviceEndpointPolicyName, cancellationToken);
            return CreateDelete(originalResponse, () => RestClient.CreateDeleteRequest(resourceGroupName, serviceEndpointPolicyName));
        }

        /// <summary> Creates or updates a service Endpoint Policies. </summary>
        /// <param name="originalResponse"> The original response from starting the operation. </param>
        /// <param name="createOriginalHttpMessage"> Creates the HTTP message used for the original request. </param>
        internal Operation<ServiceEndpointPolicy> CreateCreateOrUpdate(Response originalResponse, Func<HttpMessage> createOriginalHttpMessage)
        {
            if (originalResponse == null)
            {
                throw new ArgumentNullException(nameof(originalResponse));
            }
            if (createOriginalHttpMessage == null)
            {
                throw new ArgumentNullException(nameof(createOriginalHttpMessage));
            }

            return ArmOperationHelpers.Create(_pipeline, _clientDiagnostics, originalResponse, RequestMethod.Put, "ServiceEndpointPoliciesClient.CreateOrUpdate", OperationFinalStateVia.AzureAsyncOperation, createOriginalHttpMessage,
            (response, cancellationToken) =>
            {
                using var document = JsonDocument.Parse(response.ContentStream);
                if (document.RootElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    return ServiceEndpointPolicy.DeserializeServiceEndpointPolicy(document.RootElement);
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
                    return ServiceEndpointPolicy.DeserializeServiceEndpointPolicy(document.RootElement);
                }
            });
        }

        /// <summary> Creates or updates a service Endpoint Policies. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="parameters"> Parameters supplied to the create or update service endpoint policy operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async ValueTask<Operation<ServiceEndpointPolicy>> StartCreateOrUpdateAsync(string resourceGroupName, string serviceEndpointPolicyName, ServiceEndpointPolicy parameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (serviceEndpointPolicyName == null)
            {
                throw new ArgumentNullException(nameof(serviceEndpointPolicyName));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var originalResponse = await RestClient.CreateOrUpdateAsync(resourceGroupName, serviceEndpointPolicyName, parameters, cancellationToken).ConfigureAwait(false);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, serviceEndpointPolicyName, parameters));
        }

        /// <summary> Creates or updates a service Endpoint Policies. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. </param>
        /// <param name="serviceEndpointPolicyName"> The name of the service endpoint policy. </param>
        /// <param name="parameters"> Parameters supplied to the create or update service endpoint policy operation. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Operation<ServiceEndpointPolicy> StartCreateOrUpdate(string resourceGroupName, string serviceEndpointPolicyName, ServiceEndpointPolicy parameters, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (serviceEndpointPolicyName == null)
            {
                throw new ArgumentNullException(nameof(serviceEndpointPolicyName));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var originalResponse = RestClient.CreateOrUpdate(resourceGroupName, serviceEndpointPolicyName, parameters, cancellationToken);
            return CreateCreateOrUpdate(originalResponse, () => RestClient.CreateCreateOrUpdateRequest(resourceGroupName, serviceEndpointPolicyName, parameters));
        }
    }
}
