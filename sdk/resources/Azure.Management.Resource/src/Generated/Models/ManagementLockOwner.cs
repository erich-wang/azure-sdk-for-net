// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Management.Resource.Models
{
    /// <summary> Lock owner properties. </summary>
    public partial class ManagementLockOwner
    {
        /// <summary> Initializes a new instance of ManagementLockOwner. </summary>
        public ManagementLockOwner()
        {
        }

        /// <summary> Initializes a new instance of ManagementLockOwner. </summary>
        /// <param name="applicationId"> The application ID of the lock owner. </param>
        internal ManagementLockOwner(string applicationId)
        {
            ApplicationId = applicationId;
        }

        /// <summary> The application ID of the lock owner. </summary>
        public string ApplicationId { get; set; }
    }
}
