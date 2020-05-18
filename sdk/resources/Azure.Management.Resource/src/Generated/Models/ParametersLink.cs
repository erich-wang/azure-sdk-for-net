// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace Azure.Management.Resource.Models
{
    /// <summary> Entity representing the reference to the deployment parameters. </summary>
    public partial class ParametersLink
    {
        /// <summary> Initializes a new instance of ParametersLink. </summary>
        /// <param name="uri"> The URI of the parameters file. </param>
        public ParametersLink(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            Uri = uri;
        }

        /// <summary> Initializes a new instance of ParametersLink. </summary>
        /// <param name="uri"> The URI of the parameters file. </param>
        /// <param name="contentVersion"> If included, must match the ContentVersion in the template. </param>
        internal ParametersLink(string uri, string contentVersion)
        {
            Uri = uri;
            ContentVersion = contentVersion;
        }

        /// <summary> The URI of the parameters file. </summary>
        public string Uri { get; set; }
        /// <summary> If included, must match the ContentVersion in the template. </summary>
        public string ContentVersion { get; set; }
    }
}
