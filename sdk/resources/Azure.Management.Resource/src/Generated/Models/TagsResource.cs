// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace Azure.Management.Resource.Models
{
    /// <summary> Wrapper resource for tags API requests and responses. </summary>
    public partial class TagsResource
    {
        /// <summary> Initializes a new instance of TagsResource. </summary>
        /// <param name="properties"> The set of tags. </param>
        public TagsResource(Tags properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Properties = properties;
        }

        /// <summary> Initializes a new instance of TagsResource. </summary>
        /// <param name="id"> The ID of the tags wrapper resource. </param>
        /// <param name="name"> The name of the tags wrapper resource. </param>
        /// <param name="type"> The type of the tags wrapper resource. </param>
        /// <param name="properties"> The set of tags. </param>
        internal TagsResource(string id, string name, string type, Tags properties)
        {
            Id = id;
            Name = name;
            Type = type;
            Properties = properties;
        }

        /// <summary> The ID of the tags wrapper resource. </summary>
        public string Id { get; }
        /// <summary> The name of the tags wrapper resource. </summary>
        public string Name { get; }
        /// <summary> The type of the tags wrapper resource. </summary>
        public string Type { get; }
        /// <summary> The set of tags. </summary>
        public Tags Properties { get; set; }
    }
}
