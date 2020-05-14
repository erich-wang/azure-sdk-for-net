// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;

namespace Azure.Management.Resource.Models
{
    /// <summary> Export resource group template request parameters. </summary>
    public partial class ExportTemplateRequest
    {
        /// <summary> Initializes a new instance of ExportTemplateRequest. </summary>
        public ExportTemplateRequest()
        {
        }

        /// <summary> Initializes a new instance of ExportTemplateRequest. </summary>
        /// <param name="resources"> The IDs of the resources to filter the export by. To export all resources, supply an array with single entry &apos;*&apos;. </param>
        /// <param name="options"> The export template options. A CSV-formatted list containing zero or more of the following: &apos;IncludeParameterDefaultValue&apos;, &apos;IncludeComments&apos;, &apos;SkipResourceNameParameterization&apos;, &apos;SkipAllParameterization&apos;. </param>
        internal ExportTemplateRequest(IList<string> resources, string options)
        {
            Resources = resources;
            Options = options;
        }

        /// <summary> The IDs of the resources to filter the export by. To export all resources, supply an array with single entry &apos;*&apos;. </summary>
        public IList<string> Resources { get; set; }
        /// <summary> The export template options. A CSV-formatted list containing zero or more of the following: &apos;IncludeParameterDefaultValue&apos;, &apos;IncludeComments&apos;, &apos;SkipResourceNameParameterization&apos;, &apos;SkipAllParameterization&apos;. </summary>
        public string Options { get; set; }
    }
}
