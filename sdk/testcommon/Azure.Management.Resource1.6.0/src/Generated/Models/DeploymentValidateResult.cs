// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Management.Resource.Models
{
    /// <summary> Information from validate template deployment response. </summary>
    public partial class DeploymentValidateResult
    {
        /// <summary> Initializes a new instance of DeploymentValidateResult. </summary>
        internal DeploymentValidateResult()
        {
        }

        /// <summary> Initializes a new instance of DeploymentValidateResult. </summary>
        /// <param name="error"> Validation error. </param>
        /// <param name="properties"> The template deployment properties. </param>
        internal DeploymentValidateResult(ResourceManagementErrorWithDetails error, DeploymentPropertiesExtended properties)
        {
            Error = error;
            Properties = properties;
        }

        /// <summary> Validation error. </summary>
        public ResourceManagementErrorWithDetails Error { get; }
        /// <summary> The template deployment properties. </summary>
        public DeploymentPropertiesExtended Properties { get; }
    }
}
