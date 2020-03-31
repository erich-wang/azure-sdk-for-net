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
using Sample.Models;

namespace Sample
{
    internal partial class SuppressionsRestClient
    {
        private string subscriptionId;
        private string host;
        private string apiVersion;
        private ClientDiagnostics clientDiagnostics;
        private HttpPipeline pipeline;

        /// <summary> Initializes a new instance of SuppressionsRestClient. </summary>
        public SuppressionsRestClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2020-01-01")
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
            this.clientDiagnostics = clientDiagnostics;
            this.pipeline = pipeline;
        }

        internal HttpMessage CreateGetRequest(string resourceUri, string recommendationId, string name)
        {
            var message = pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/", false);
            uri.AppendPath(resourceUri, true);
            uri.AppendPath("/providers/Microsoft.Advisor/recommendations/", false);
            uri.AppendPath(recommendationId, true);
            uri.AppendPath("/suppressions/", false);
            uri.AppendPath(name, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Obtains the details of a suppression. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<SuppressionContract>> GetAsync(string resourceUri, string recommendationId, string name, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Get");
            scope.Start();
            try
            {
                using var message = CreateGetRequest(resourceUri, recommendationId, name);
                await pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContract value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            value = SuppressionContract.DeserializeSuppressionContract(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Obtains the details of a suppression. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<SuppressionContract> Get(string resourceUri, string recommendationId, string name, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Get");
            scope.Start();
            try
            {
                using var message = CreateGetRequest(resourceUri, recommendationId, name);
                pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContract value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            value = SuppressionContract.DeserializeSuppressionContract(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal HttpMessage CreateCreateRequest(string resourceUri, string recommendationId, string name, SuppressionContract suppressionContract)
        {
            var message = pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Put;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/", false);
            uri.AppendPath(resourceUri, true);
            uri.AppendPath("/providers/Microsoft.Advisor/recommendations/", false);
            uri.AppendPath(recommendationId, true);
            uri.AppendPath("/suppressions/", false);
            uri.AppendPath(name, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            request.Headers.Add("Content-Type", "application/json");
            using var content = new Utf8JsonRequestContent();
            content.JsonWriter.WriteObjectValue(suppressionContract);
            request.Content = content;
            return message;
        }

        /// <summary> Enables the snoozed or dismissed attribute of a recommendation. The snoozed or dismissed attribute is referred to as a suppression. Use this API to create or update the snoozed or dismissed status of a recommendation. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="suppressionContract"> The snoozed or dismissed attribute; for example, the snooze duration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<SuppressionContract>> CreateAsync(string resourceUri, string recommendationId, string name, SuppressionContract suppressionContract, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (suppressionContract == null)
            {
                throw new ArgumentNullException(nameof(suppressionContract));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Create");
            scope.Start();
            try
            {
                using var message = CreateCreateRequest(resourceUri, recommendationId, name, suppressionContract);
                await pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContract value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            value = SuppressionContract.DeserializeSuppressionContract(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Enables the snoozed or dismissed attribute of a recommendation. The snoozed or dismissed attribute is referred to as a suppression. Use this API to create or update the snoozed or dismissed status of a recommendation. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="suppressionContract"> The snoozed or dismissed attribute; for example, the snooze duration. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<SuppressionContract> Create(string resourceUri, string recommendationId, string name, SuppressionContract suppressionContract, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (suppressionContract == null)
            {
                throw new ArgumentNullException(nameof(suppressionContract));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Create");
            scope.Start();
            try
            {
                using var message = CreateCreateRequest(resourceUri, recommendationId, name, suppressionContract);
                pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContract value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            value = SuppressionContract.DeserializeSuppressionContract(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal HttpMessage CreateDeleteRequest(string resourceUri, string recommendationId, string name)
        {
            var message = pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/", false);
            uri.AppendPath(resourceUri, true);
            uri.AppendPath("/providers/Microsoft.Advisor/recommendations/", false);
            uri.AppendPath(recommendationId, true);
            uri.AppendPath("/suppressions/", false);
            uri.AppendPath(name, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Enables the activation of a snoozed or dismissed recommendation. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response> DeleteAsync(string resourceUri, string recommendationId, string name, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Delete");
            scope.Start();
            try
            {
                using var message = CreateDeleteRequest(resourceUri, recommendationId, name);
                await pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 204:
                        return message.Response;
                    default:
                        throw await clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Enables the activation of a snoozed or dismissed recommendation. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="resourceUri"> The fully qualified Azure Resource Manager identifier of the resource to which the recommendation applies. </param>
        /// <param name="recommendationId"> The recommendation ID. </param>
        /// <param name="name"> The name of the suppression. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response Delete(string resourceUri, string recommendationId, string name, CancellationToken cancellationToken = default)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }
            if (recommendationId == null)
            {
                throw new ArgumentNullException(nameof(recommendationId));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.Delete");
            scope.Start();
            try
            {
                using var message = CreateDeleteRequest(resourceUri, recommendationId, name);
                pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 204:
                        return message.Response;
                    default:
                        throw clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal HttpMessage CreateListRequest(int? top, string skipToken)
        {
            var message = pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/providers/Microsoft.Advisor/suppressions", false);
            uri.AppendQuery("api-version", apiVersion, true);
            if (top != null)
            {
                uri.AppendQuery("$top", top.Value, true);
            }
            if (skipToken != null)
            {
                uri.AppendQuery("$skipToken", skipToken, true);
            }
            request.Uri = uri;
            return message;
        }

        /// <summary> Retrieves the list of snoozed or dismissed suppressions for a subscription. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="top"> The number of suppressions per page if a paged version of this API is being used. </param>
        /// <param name="skipToken"> The page-continuation token to use with a paged version of this API. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<SuppressionContractListResult>> ListAsync(int? top, string skipToken, CancellationToken cancellationToken = default)
        {
            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest(top, skipToken);
                await pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContractListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            value = SuppressionContractListResult.DeserializeSuppressionContractListResult(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Retrieves the list of snoozed or dismissed suppressions for a subscription. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="top"> The number of suppressions per page if a paged version of this API is being used. </param>
        /// <param name="skipToken"> The page-continuation token to use with a paged version of this API. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<SuppressionContractListResult> List(int? top, string skipToken, CancellationToken cancellationToken = default)
        {
            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest(top, skipToken);
                pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContractListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            value = SuppressionContractListResult.DeserializeSuppressionContractListResult(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal HttpMessage CreateListNextPageRequest(string nextLink)
        {
            var message = pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(nextLink, false);
            request.Uri = uri;
            return message;
        }

        /// <summary> Retrieves the list of snoozed or dismissed suppressions for a subscription. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<SuppressionContractListResult>> ListNextPageAsync(string nextLink, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink);
                await pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContractListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            value = SuppressionContractListResult.DeserializeSuppressionContractListResult(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Retrieves the list of snoozed or dismissed suppressions for a subscription. The snoozed or dismissed attribute of a recommendation is referred to as a suppression. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<SuppressionContractListResult> ListNextPage(string nextLink, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var scope = clientDiagnostics.CreateScope("SuppressionsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink);
                pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            SuppressionContractListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            value = SuppressionContractListResult.DeserializeSuppressionContractListResult(document.RootElement);
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }
    }
}
