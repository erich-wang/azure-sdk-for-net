// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace Azure.Management.Resource.Models
{
    public partial class ExportTemplateRequest : IUtf8JsonSerializable
    {
        void IUtf8JsonSerializable.Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            if (Resources != null)
            {
                writer.WritePropertyName("resources");
                writer.WriteStartArray();
                foreach (var item in Resources)
                {
                    writer.WriteStringValue(item);
                }
                writer.WriteEndArray();
            }
            if (Options != null)
            {
                writer.WritePropertyName("options");
                writer.WriteStringValue(Options);
            }
            writer.WriteEndObject();
        }
    }
}
