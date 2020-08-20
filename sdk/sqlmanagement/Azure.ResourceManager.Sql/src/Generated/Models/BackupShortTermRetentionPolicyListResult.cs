// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace Azure.ResourceManager.Sql.Models
{
    /// <summary> A list of short term retention policies. </summary>
    public partial class BackupShortTermRetentionPolicyListResult
    {
        /// <summary> Initializes a new instance of BackupShortTermRetentionPolicyListResult. </summary>
        internal BackupShortTermRetentionPolicyListResult()
        {
            Value = new ChangeTrackingList<BackupShortTermRetentionPolicy>();
        }

        /// <summary> Initializes a new instance of BackupShortTermRetentionPolicyListResult. </summary>
        /// <param name="value"> Array of results. </param>
        /// <param name="nextLink"> Link to retrieve next page of results. </param>
        internal BackupShortTermRetentionPolicyListResult(IReadOnlyList<BackupShortTermRetentionPolicy> value, string nextLink)
        {
            Value = value;
            NextLink = nextLink;
        }

        /// <summary> Array of results. </summary>
        public IReadOnlyList<BackupShortTermRetentionPolicy> Value { get; }
        /// <summary> Link to retrieve next page of results. </summary>
        public string NextLink { get; }
    }
}