// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Management.Resource.Models
{
    /// <summary> Resource group export result. </summary>
    public partial class ResourceGroupExportResult
    {
        /// <summary> Initializes a new instance of ResourceGroupExportResult. </summary>
        internal ResourceGroupExportResult()
        {
        }

        /// <summary> Initializes a new instance of ResourceGroupExportResult. </summary>
        /// <param name="template"> The template content. </param>
        /// <param name="error"> The template export error. </param>
        internal ResourceGroupExportResult(object template, ErrorResponse error)
        {
            Template = template;
            Error = error;
        }

        /// <summary> The template content. </summary>
        public object Template { get; }
        /// <summary> The template export error. </summary>
        public ErrorResponse Error { get; }
    }
}
