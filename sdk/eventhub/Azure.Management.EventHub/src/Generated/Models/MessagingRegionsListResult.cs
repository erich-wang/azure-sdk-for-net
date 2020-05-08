// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;

namespace Azure.Management.EventHub.Models
{
    /// <summary> The response of the List MessagingRegions operation. </summary>
    public partial class MessagingRegionsListResult
    {
        /// <summary> Initializes a new instance of MessagingRegionsListResult. </summary>
        internal MessagingRegionsListResult()
        {
        }

        /// <summary> Initializes a new instance of MessagingRegionsListResult. </summary>
        /// <param name="value"> Result of the List MessagingRegions type. </param>
        /// <param name="nextLink"> Link to the next set of results. Not empty if Value contains incomplete list of MessagingRegions. </param>
        internal MessagingRegionsListResult(IReadOnlyList<MessagingRegions> value, string nextLink)
        {
            Value = value;
            NextLink = nextLink;
        }

        /// <summary> Result of the List MessagingRegions type. </summary>
        public IReadOnlyList<MessagingRegions> Value { get; }
        /// <summary> Link to the next set of results. Not empty if Value contains incomplete list of MessagingRegions. </summary>
        public string NextLink { get; }
    }
}
