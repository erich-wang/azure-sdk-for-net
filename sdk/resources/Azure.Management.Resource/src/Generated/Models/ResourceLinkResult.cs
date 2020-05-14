// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Management.Resource.Models
{
    /// <summary> List of resource links. </summary>
    public partial class ResourceLinkResult
    {
        /// <summary> Initializes a new instance of ResourceLinkResult. </summary>
        /// <param name="value"> An array of resource links. </param>
        internal ResourceLinkResult(IEnumerable<ResourceLink> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value.ToArray();
        }

        /// <summary> Initializes a new instance of ResourceLinkResult. </summary>
        /// <param name="value"> An array of resource links. </param>
        /// <param name="nextLink"> The URL to use for getting the next set of results. </param>
        internal ResourceLinkResult(IReadOnlyList<ResourceLink> value, string nextLink)
        {
            Value = value;
            NextLink = nextLink;
        }

        /// <summary> An array of resource links. </summary>
        public IReadOnlyList<ResourceLink> Value { get; }
        /// <summary> The URL to use for getting the next set of results. </summary>
        public string NextLink { get; }
    }
}
