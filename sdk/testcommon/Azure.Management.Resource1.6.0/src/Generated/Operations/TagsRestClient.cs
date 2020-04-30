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
    internal partial class TagsRestClient
    {
        private string subscriptionId;
        private string host;
        private string apiVersion;
        private ClientDiagnostics _clientDiagnostics;
        private HttpPipeline _pipeline;

        /// <summary> Initializes a new instance of TagsRestClient. </summary>
        public TagsRestClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2017-05-10")
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

        internal Core.HttpMessage CreateDeleteValueRequest(string tagName, string tagValue)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/tagNames/", false);
            uri.AppendPath(tagName, true);
            uri.AppendPath("/tagValues/", false);
            uri.AppendPath(tagValue, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Deletes a tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag to delete. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response> DeleteValueAsync(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }
            if (tagValue == null)
            {
                throw new ArgumentNullException(nameof(tagValue));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.DeleteValue");
            scope.Start();
            try
            {
                using var message = CreateDeleteValueRequest(tagName, tagValue);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                    case 204:
                        return message.Response;
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Deletes a tag value. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag to delete. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response DeleteValue(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }
            if (tagValue == null)
            {
                throw new ArgumentNullException(nameof(tagValue));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.DeleteValue");
            scope.Start();
            try
            {
                using var message = CreateDeleteValueRequest(tagName, tagValue);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                    case 204:
                        return message.Response;
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal Core.HttpMessage CreateCreateOrUpdateValueRequest(string tagName, string tagValue)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Put;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/tagNames/", false);
            uri.AppendPath(tagName, true);
            uri.AppendPath("/tagValues/", false);
            uri.AppendPath(tagValue, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Creates a tag value. The name of the tag must already exist. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag to create. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<TagValue>> CreateOrUpdateValueAsync(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }
            if (tagValue == null)
            {
                throw new ArgumentNullException(nameof(tagValue));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.CreateOrUpdateValue");
            scope.Start();
            try
            {
                using var message = CreateCreateOrUpdateValueRequest(tagName, tagValue);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                    case 201:
                        {
                            TagValue value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagValue.DeserializeTagValue(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Creates a tag value. The name of the tag must already exist. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="tagValue"> The value of the tag to create. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<TagValue> CreateOrUpdateValue(string tagName, string tagValue, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }
            if (tagValue == null)
            {
                throw new ArgumentNullException(nameof(tagValue));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.CreateOrUpdateValue");
            scope.Start();
            try
            {
                using var message = CreateCreateOrUpdateValueRequest(tagName, tagValue);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                    case 201:
                        {
                            TagValue value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagValue.DeserializeTagValue(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal Core.HttpMessage CreateCreateOrUpdateRequest(string tagName)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Put;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/tagNames/", false);
            uri.AppendPath(tagName, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> The tag name can have a maximum of 512 characters and is case insensitive. Tag names created by Azure have prefixes of microsoft, azure, or windows. You cannot create tags with one of these prefixes. </summary>
        /// <param name="tagName"> The name of the tag to create. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<TagDetails>> CreateOrUpdateAsync(string tagName, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.CreateOrUpdate");
            scope.Start();
            try
            {
                using var message = CreateCreateOrUpdateRequest(tagName);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                    case 201:
                        {
                            TagDetails value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagDetails.DeserializeTagDetails(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> The tag name can have a maximum of 512 characters and is case insensitive. Tag names created by Azure have prefixes of microsoft, azure, or windows. You cannot create tags with one of these prefixes. </summary>
        /// <param name="tagName"> The name of the tag to create. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<TagDetails> CreateOrUpdate(string tagName, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.CreateOrUpdate");
            scope.Start();
            try
            {
                using var message = CreateCreateOrUpdateRequest(tagName);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                    case 201:
                        {
                            TagDetails value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagDetails.DeserializeTagDetails(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal Core.HttpMessage CreateDeleteRequest(string tagName)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/tagNames/", false);
            uri.AppendPath(tagName, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> You must remove all values from a resource tag before you can delete it. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response> DeleteAsync(string tagName, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.Delete");
            scope.Start();
            try
            {
                using var message = CreateDeleteRequest(tagName);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                    case 204:
                        return message.Response;
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> You must remove all values from a resource tag before you can delete it. </summary>
        /// <param name="tagName"> The name of the tag. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response Delete(string tagName, CancellationToken cancellationToken = default)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.Delete");
            scope.Start();
            try
            {
                using var message = CreateDeleteRequest(tagName);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                    case 204:
                        return message.Response;
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal Core.HttpMessage CreateListRequest()
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/tagNames", false);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets the names and values of all resource tags that are defined in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<TagsListResult>> ListAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("TagsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest();
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            TagsListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagsListResult.DeserializeTagsListResult(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Gets the names and values of all resource tags that are defined in a subscription. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<TagsListResult> List(CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("TagsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest();
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            TagsListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagsListResult.DeserializeTagsListResult(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        internal Core.HttpMessage CreateListNextPageRequest(string nextLink)
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

        /// <summary> Gets the names and values of all resource tags that are defined in a subscription. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<TagsListResult>> ListNextPageAsync(string nextLink, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            TagsListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagsListResult.DeserializeTagsListResult(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(message.Response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Gets the names and values of all resource tags that are defined in a subscription. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<TagsListResult> ListNextPage(string nextLink, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }

            using var scope = _clientDiagnostics.CreateScope("TagsClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            TagsListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = TagsListResult.DeserializeTagsListResult(document.RootElement);
                            }
                            return Response.FromValue(value, message.Response);
                        }
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(message.Response);
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
