// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace Azure.Management.Resource.Models
{
    /// <summary> Deployment What-if operation parameters. </summary>
    public partial class DeploymentWhatIf
    {
        /// <summary> Initializes a new instance of DeploymentWhatIf. </summary>
        /// <param name="properties"> The deployment properties. </param>
        public DeploymentWhatIf(DeploymentWhatIfProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Properties = properties;
        }

        /// <summary> Initializes a new instance of DeploymentWhatIf. </summary>
        /// <param name="location"> The location to store the deployment data. </param>
        /// <param name="properties"> The deployment properties. </param>
        internal DeploymentWhatIf(string location, DeploymentWhatIfProperties properties)
        {
            Location = location;
            Properties = properties;
        }

        /// <summary> The location to store the deployment data. </summary>
        public string Location { get; set; }
        /// <summary> The deployment properties. </summary>
        public DeploymentWhatIfProperties Properties { get; }
    }
}
