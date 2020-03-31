// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Pipeline;
using Azure.Management.Storage.Models;

namespace Azure.Management.Storage
{
    public partial class ManagementPoliciesClient
    {
        private readonly ClientDiagnostics clientDiagnostics;
        private readonly HttpPipeline pipeline;
        internal ManagementPoliciesRestClient RestClient { get; }
        /// <summary> Initializes a new instance of ManagementPoliciesClient for mocking. </summary>
        protected ManagementPoliciesClient()
        {
        }
        /// <summary> Initializes a new instance of ManagementPoliciesClient. </summary>
        internal ManagementPoliciesClient(ClientDiagnostics clientDiagnostics, HttpPipeline pipeline, string subscriptionId, string host = "https://management.azure.com", string apiVersion = "2019-06-01")
        {
            RestClient = new ManagementPoliciesRestClient(clientDiagnostics, pipeline, subscriptionId, host, apiVersion);
            this.clientDiagnostics = clientDiagnostics;
            this.pipeline = pipeline;
        }

        /// <summary> Gets the managementpolicy associated with the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ManagementPolicy>> GetAsync(string resourceGroupName, string accountName, CancellationToken cancellationToken = default)
        {
            return await RestClient.GetAsync(resourceGroupName, accountName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets the managementpolicy associated with the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ManagementPolicy> Get(string resourceGroupName, string accountName, CancellationToken cancellationToken = default)
        {
            return RestClient.Get(resourceGroupName, accountName, cancellationToken);
        }

        /// <summary> Sets the managementpolicy to the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="properties"> The ManagementPolicy set to a storage account. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<ManagementPolicy>> CreateOrUpdateAsync(string resourceGroupName, string accountName, ManagementPolicy properties, CancellationToken cancellationToken = default)
        {
            return await RestClient.CreateOrUpdateAsync(resourceGroupName, accountName, properties, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Sets the managementpolicy to the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="properties"> The ManagementPolicy set to a storage account. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<ManagementPolicy> CreateOrUpdate(string resourceGroupName, string accountName, ManagementPolicy properties, CancellationToken cancellationToken = default)
        {
            return RestClient.CreateOrUpdate(resourceGroupName, accountName, properties, cancellationToken);
        }

        /// <summary> Deletes the managementpolicy associated with the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response> DeleteAsync(string resourceGroupName, string accountName, CancellationToken cancellationToken = default)
        {
            return await RestClient.DeleteAsync(resourceGroupName, accountName, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Deletes the managementpolicy associated with the specified storage account. </summary>
        /// <param name="resourceGroupName"> The name of the resource group within the user&apos;s subscription. The name is case insensitive. </param>
        /// <param name="accountName"> The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response Delete(string resourceGroupName, string accountName, CancellationToken cancellationToken = default)
        {
            return RestClient.Delete(resourceGroupName, accountName, cancellationToken);
        }
    }
}
