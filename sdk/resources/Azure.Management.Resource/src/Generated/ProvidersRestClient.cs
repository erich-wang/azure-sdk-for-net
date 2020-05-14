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
using Azure.Management.Resource.Models;

namespace Azure.Management.Resource
{
    internal partial class ProvidersRestClient
    {
        private string subscriptionId;
        private string host;
        private string apiVersion;
        private ClientDiagnostics _clientDiagnostics;
        private HttpPipeline _pipeline;

        /// <summary> Initializes a new instance of ProvidersRestClient. </summary>
        public ProvidersRestClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2019-10-01")
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (apiVersion == null)
            {
                throw new ArgumentNullException(nameof(apiVersion));
            }

            this.subscriptionId = subscriptionId;
            this.host = host;
            this.apiVersion = apiVersion;
            _clientDiagnostics = clientDiagnostics;
            _pipeline = pipeline;
        }

        internal Core.HttpMessage CreateUnregisterRequest(string resourceProviderNamespace)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/providers/", false);
            uri.AppendPath(resourceProviderNamespace, true);
            uri.AppendPath("/unregister", false);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Unregisters a subscription from a resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider to unregister. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<Provider>> UnregisterAsync(string resourceProviderNamespace, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateUnregisterRequest(resourceProviderNamespace);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Unregisters a subscription from a resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider to unregister. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<Provider> Unregister(string resourceProviderNamespace, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateUnregisterRequest(resourceProviderNamespace);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateRegisterRequest(string resourceProviderNamespace)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/providers/", false);
            uri.AppendPath(resourceProviderNamespace, true);
            uri.AppendPath("/register", false);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Registers a subscription with a resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider to register. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<Provider>> RegisterAsync(string resourceProviderNamespace, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateRegisterRequest(resourceProviderNamespace);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Registers a subscription with a resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider to register. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<Provider> Register(string resourceProviderNamespace, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateRegisterRequest(resourceProviderNamespace);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateListRequest(int? top, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/providers", false);
            if (top != null)
            {
                uri.AppendQuery("$top", top.Value, true);
            }
            if (expand != null)
            {
                uri.AppendQuery("$expand", expand, true);
            }
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets all resource providers for a subscription. </summary>
        /// <param name="top"> The number of results to return. If null is passed returns all deployments. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<ProviderListResult>> ListAsync(int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateListRequest(top, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets all resource providers for a subscription. </summary>
        /// <param name="top"> The number of results to return. If null is passed returns all deployments. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<ProviderListResult> List(int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateListRequest(top, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateListAtTenantScopeRequest(int? top, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/providers", false);
            if (top != null)
            {
                uri.AppendQuery("$top", top.Value, true);
            }
            if (expand != null)
            {
                uri.AppendQuery("$expand", expand, true);
            }
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets all resource providers for the tenant. </summary>
        /// <param name="top"> The number of results to return. If null is passed returns all providers. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<ProviderListResult>> ListAtTenantScopeAsync(int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateListAtTenantScopeRequest(top, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets all resource providers for the tenant. </summary>
        /// <param name="top"> The number of results to return. If null is passed returns all providers. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<ProviderListResult> ListAtTenantScope(int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateListAtTenantScopeRequest(top, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateGetRequest(string resourceProviderNamespace, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/providers/", false);
            uri.AppendPath(resourceProviderNamespace, true);
            if (expand != null)
            {
                uri.AppendQuery("$expand", expand, true);
            }
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets the specified resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider. </param>
        /// <param name="expand"> The $expand query parameter. For example, to include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<Provider>> GetAsync(string resourceProviderNamespace, string expand = null, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateGetRequest(resourceProviderNamespace, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets the specified resource provider. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider. </param>
        /// <param name="expand"> The $expand query parameter. For example, to include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<Provider> Get(string resourceProviderNamespace, string expand = null, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateGetRequest(resourceProviderNamespace, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateGetAtTenantScopeRequest(string resourceProviderNamespace, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/providers/", false);
            uri.AppendPath(resourceProviderNamespace, true);
            if (expand != null)
            {
                uri.AppendQuery("$expand", expand, true);
            }
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets the specified resource provider at the tenant level. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider. </param>
        /// <param name="expand"> The $expand query parameter. For example, to include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<Provider>> GetAtTenantScopeAsync(string resourceProviderNamespace, string expand = null, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateGetAtTenantScopeRequest(resourceProviderNamespace, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets the specified resource provider at the tenant level. </summary>
        /// <param name="resourceProviderNamespace"> The namespace of the resource provider. </param>
        /// <param name="expand"> The $expand query parameter. For example, to include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<Provider> GetAtTenantScope(string resourceProviderNamespace, string expand = null, CancellationToken cancellationToken = default)
        {
            if (resourceProviderNamespace == null)
            {
                throw new ArgumentNullException(nameof(resourceProviderNamespace));
            }

            using var message = CreateGetAtTenantScopeRequest(resourceProviderNamespace, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        Provider value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = Provider.DeserializeProvider(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateListNextPageRequest(string nextLink, int? top, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendRawNextLink(nextLink, false);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets all resource providers for a subscription. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="top"> The number of results to return. If null is passed returns all deployments. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<ProviderListResult>> ListNextPageAsync(string nextLink, int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var message = CreateListNextPageRequest(nextLink, top, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets all resource providers for a subscription. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="top"> The number of results to return. If null is passed returns all deployments. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<ProviderListResult> ListNextPage(string nextLink, int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var message = CreateListNextPageRequest(nextLink, top, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }

        internal Core.HttpMessage CreateListAtTenantScopeNextPageRequest(string nextLink, int? top, string expand)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendRawNextLink(nextLink, false);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets all resource providers for the tenant. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="top"> The number of results to return. If null is passed returns all providers. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<ProviderListResult>> ListAtTenantScopeNextPageAsync(string nextLink, int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var message = CreateListAtTenantScopeNextPageRequest(nextLink, top, expand);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
            }
        }

        /// <summary> Gets all resource providers for the tenant. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="top"> The number of results to return. If null is passed returns all providers. </param>
        /// <param name="expand"> The properties to include in the results. For example, use &amp;$expand=metadata in the query string to retrieve resource provider metadata. To include property aliases in response, use $expand=resourceTypes/aliases. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<ProviderListResult> ListAtTenantScopeNextPage(string nextLink, int? top = null, string expand = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var message = CreateListAtTenantScopeNextPageRequest(nextLink, top, expand);
            _pipeline.Send(message, cancellationToken);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        ProviderListResult value = default;
                        using var document = JsonDocument.Parse(message.Response.ContentStream);
                        if (document.RootElement.ValueKind == JsonValueKind.Null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = ProviderListResult.DeserializeProviderListResult(document.RootElement);
                        }
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw _clientDiagnostics.CreateRequestFailedException(message.Response);
            }
        }
    }
}
