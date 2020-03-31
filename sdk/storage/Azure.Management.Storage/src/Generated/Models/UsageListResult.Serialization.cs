// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.Management.Storage.Models
{
    public partial class UsageListResult
    {
        internal static UsageListResult DeserializeUsageListResult(JsonElement element)
        {
            IReadOnlyList<Usage> value = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("value"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<Usage> array = new List<Usage>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(Usage.DeserializeUsage(item));
                    }
                    value = array;
                    continue;
                }
            }
            return new UsageListResult(value);
        }
    }
}
