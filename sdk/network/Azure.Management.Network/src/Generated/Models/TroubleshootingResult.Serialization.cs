// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.Management.Network.Models
{
    public partial class TroubleshootingResult
    {
        internal static TroubleshootingResult DeserializeTroubleshootingResult(JsonElement element)
        {
            DateTimeOffset? startTime = default;
            DateTimeOffset? endTime = default;
            string code = default;
            IReadOnlyList<TroubleshootingDetails> results = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("startTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    startTime = property.Value.GetDateTimeOffset("S");
                    continue;
                }
                if (property.NameEquals("endTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    endTime = property.Value.GetDateTimeOffset("S");
                    continue;
                }
                if (property.NameEquals("code"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    code = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("results"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<TroubleshootingDetails> array = new List<TroubleshootingDetails>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Null)
                        {
                            array.Add(null);
                        }
                        else
                        {
                            array.Add(TroubleshootingDetails.DeserializeTroubleshootingDetails(item));
                        }
                    }
                    results = array;
                    continue;
                }
            }
            return new TroubleshootingResult(startTime, endTime, code, results);
        }
    }
}
