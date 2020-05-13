// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Xml;
using Azure.Core.Pipeline;

namespace Azure.Core
{
    internal static class ManagementPipelineBuilder
    {
        public static HttpPipeline Build(TokenCredential credential, ClientOptions options)
        {
            return HttpPipelineBuilder.Build(options, new BearerTokenAuthenticationPolicy(credential, "https://management.azure.com//.default"));
        }

        public static HttpPipeline Build(TokenCredential credential, string host, ClientOptions options)
        {
            return HttpPipelineBuilder.Build(options, new BearerTokenAuthenticationPolicy(credential, $"{host}//.default"));
        }
    }
}
