// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.Network.Models;

namespace Azure.Management.Network
{
    /// <summary> The ServiceTags service client. </summary>
    public partial class ServiceTagsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal ServiceTagsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ServiceTagsClient for mocking. </summary>
        protected ServiceTagsClient()
        {
        }

        /// <summary> Initializes a new instance of ServiceTagsClient. </summary>
        public ServiceTagsClient(string subscriptionId, TokenCredential tokenCredential, NetworkManagementClientOptions options = null)
        {
            options ??= new NetworkManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new ServiceTagsRestClient(_clientDiagnostics, _pipeline, subscriptionId: subscriptionId);
        }

        /// <summary> Gets a list of service tag information resources. </summary>
        /// <param name="location"> The location that will be used as a reference for version (not as a filter based on location, you will get the list of service tags with prefix details across all regions but limited to the cloud that your subscription belongs to). </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ServiceTagsListResult>> ListAsync(string location, CancellationToken cancellationToken = default)
        {
            return await RestClient.ListAsync(location, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets a list of service tag information resources. </summary>
        /// <param name="location"> The location that will be used as a reference for version (not as a filter based on location, you will get the list of service tags with prefix details across all regions but limited to the cloud that your subscription belongs to). </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ServiceTagsListResult> List(string location, CancellationToken cancellationToken = default)
        {
            return RestClient.List(location, cancellationToken);
        }
    }
}
