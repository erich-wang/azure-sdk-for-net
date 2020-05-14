// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;

namespace Azure.Management.Resource.Models
{
    /// <summary> Resource group information. </summary>
    public partial class ResourceGroupPatchable
    {
        /// <summary> Initializes a new instance of ResourceGroupPatchable. </summary>
        public ResourceGroupPatchable()
        {
        }

        /// <summary> Initializes a new instance of ResourceGroupPatchable. </summary>
        /// <param name="name"> The name of the resource group. </param>
        /// <param name="properties"> The resource group properties. </param>
        /// <param name="managedBy"> The ID of the resource that manages this resource group. </param>
        /// <param name="tags"> The tags attached to the resource group. </param>
        internal ResourceGroupPatchable(string name, ResourceGroupProperties properties, string managedBy, IDictionary<string, string> tags)
        {
            Name = name;
            Properties = properties;
            ManagedBy = managedBy;
            Tags = tags;
        }

        /// <summary> The name of the resource group. </summary>
        public string Name { get; set; }
        /// <summary> The resource group properties. </summary>
        public ResourceGroupProperties Properties { get; set; }
        /// <summary> The ID of the resource that manages this resource group. </summary>
        public string ManagedBy { get; set; }
        /// <summary> The tags attached to the resource group. </summary>
        public IDictionary<string, string> Tags { get; set; }
    }
}
