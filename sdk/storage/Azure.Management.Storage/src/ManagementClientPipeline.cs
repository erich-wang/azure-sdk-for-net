using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core;
using Azure.Core.Pipeline;

namespace Azure.Management.Storage
{
    internal static class ManagementClientPipeline
    {
        public static HttpPipeline Build(ClientOptions options, TokenCredential tokenCredential)
        {
            return HttpPipelineBuilder.Build(options, new BearerTokenAuthenticationPolicy(tokenCredential, "https://management.azure.com/.default"));
        }
    }
}
