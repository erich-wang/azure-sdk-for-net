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
    internal partial class DeploymentRestClient
    {
        private string subscriptionId;
        private string host;
        private string apiVersion;
        private ClientDiagnostics _clientDiagnostics;
        private HttpPipeline _pipeline;

        /// <summary> Initializes a new instance of DeploymentRestClient. </summary>
        public DeploymentRestClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2015-11-01")
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

        internal HttpMessage CreateGetRequest(string resourceGroupName, string deploymentName, string operationId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/resourcegroups/", false);
            uri.AppendPath(resourceGroupName, true);
            uri.AppendPath("/deployments/", false);
            uri.AppendPath(deploymentName, true);
            uri.AppendPath("/operations/", false);
            uri.AppendPath(operationId, true);
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Get a list of deployments operations. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="operationId"> Operation Id. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<DeploymentOperation>> GetAsync(string resourceGroupName, string deploymentName, string operationId, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }
            if (operationId == null)
            {
                throw new ArgumentNullException(nameof(operationId));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.Get");
            scope.Start();
            try
            {
                using var message = CreateGetRequest(resourceGroupName, deploymentName, operationId);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperation value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperation.DeserializeDeploymentOperation(document.RootElement);
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

        /// <summary> Get a list of deployments operations. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="operationId"> Operation Id. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<DeploymentOperation> Get(string resourceGroupName, string deploymentName, string operationId, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }
            if (operationId == null)
            {
                throw new ArgumentNullException(nameof(operationId));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.Get");
            scope.Start();
            try
            {
                using var message = CreateGetRequest(resourceGroupName, deploymentName, operationId);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperation value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperation.DeserializeDeploymentOperation(document.RootElement);
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

        internal HttpMessage CreateListRequest(string resourceGroupName, string deploymentName, int? top)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(host, false);
            uri.AppendPath("/subscriptions/", false);
            uri.AppendPath(subscriptionId, true);
            uri.AppendPath("/resourcegroups/", false);
            uri.AppendPath(resourceGroupName, true);
            uri.AppendPath("/deployments/", false);
            uri.AppendPath(deploymentName, true);
            uri.AppendPath("/operations", false);
            if (top != null)
            {
                uri.AppendQuery("$top", top.Value, true);
            }
            uri.AppendQuery("api-version", apiVersion, true);
            request.Uri = uri;
            return message;
        }

        /// <summary> Gets a list of deployments operations. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="top"> Query parameters. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<DeploymentOperationsListResult>> ListAsync(string resourceGroupName, string deploymentName, int? top = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest(resourceGroupName, deploymentName, top);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperationsListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperationsListResult.DeserializeDeploymentOperationsListResult(document.RootElement);
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

        /// <summary> Gets a list of deployments operations. </summary>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="top"> Query parameters. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<DeploymentOperationsListResult> List(string resourceGroupName, string deploymentName, int? top = null, CancellationToken cancellationToken = default)
        {
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.List");
            scope.Start();
            try
            {
                using var message = CreateListRequest(resourceGroupName, deploymentName, top);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperationsListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperationsListResult.DeserializeDeploymentOperationsListResult(document.RootElement);
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

        internal HttpMessage CreateListNextPageRequest(string nextLink, string resourceGroupName, string deploymentName, int? top)
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

        /// <summary> Gets a list of deployments operations. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="top"> Query parameters. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public async ValueTask<Response<DeploymentOperationsListResult>> ListNextPageAsync(string nextLink, string resourceGroupName, string deploymentName, int? top = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink, resourceGroupName, deploymentName, top);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperationsListResult value = default;
                            using var document = await JsonDocument.ParseAsync(message.Response.ContentStream, default, cancellationToken).ConfigureAwait(false);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperationsListResult.DeserializeDeploymentOperationsListResult(document.RootElement);
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

        /// <summary> Gets a list of deployments operations. </summary>
        /// <param name="nextLink"> The URL to the next page of results. </param>
        /// <param name="resourceGroupName"> The name of the resource group. The name is case insensitive. </param>
        /// <param name="deploymentName"> The name of the deployment. </param>
        /// <param name="top"> Query parameters. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public Response<DeploymentOperationsListResult> ListNextPage(string nextLink, string resourceGroupName, string deploymentName, int? top = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ArgumentNullException(nameof(nextLink));
            }
            if (resourceGroupName == null)
            {
                throw new ArgumentNullException(nameof(resourceGroupName));
            }
            if (deploymentName == null)
            {
                throw new ArgumentNullException(nameof(deploymentName));
            }

            using var scope = _clientDiagnostics.CreateScope("DeploymentClient.List");
            scope.Start();
            try
            {
                using var message = CreateListNextPageRequest(nextLink, resourceGroupName, deploymentName, top);
                _pipeline.Send(message, cancellationToken);
                switch (message.Response.Status)
                {
                    case 200:
                        {
                            DeploymentOperationsListResult value = default;
                            using var document = JsonDocument.Parse(message.Response.ContentStream);
                            if (document.RootElement.ValueKind == JsonValueKind.Null)
                            {
                                value = null;
                            }
                            else
                            {
                                value = DeploymentOperationsListResult.DeserializeDeploymentOperationsListResult(document.RootElement);
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
