// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Management.Resource.Models
{
    /// <summary> The identity type. This is the only required field when adding a system assigned identity to a resource. </summary>
    public enum ResourceIdentityType
    {
        /// <summary> Indicates that a system assigned identity is associated with the resource. </summary>
        SystemAssigned,
        /// <summary> Indicates that no identity is associated with the resource or that the existing identity should be removed. </summary>
        None
    }
}
