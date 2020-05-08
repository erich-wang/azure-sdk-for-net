using Azure.Core;
using Azure.Core.Pipeline;

namespace Azure.Management.EventHub
{
    public partial class EventHubsClient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "AZC0007:DO provide a minimal constructor that takes only the parameters required to connect to the service.", Justification = "<Pending>")]
        public EventHubsClient(string subscriptionId, TokenCredential tokenCredential) : this(subscriptionId, tokenCredential, EventHubManagementClientOption.Default)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "AZC0007:DO provide a minimal constructor that takes only the parameters required to connect to the service.", Justification = "<Pending>")]
        public EventHubsClient(string subscriptionId, TokenCredential tokenCredential, EventHubManagementClientOption options) :
            this(new ClientDiagnostics(options), ManagementClientPipeline.Build(options, tokenCredential), subscriptionId, apiVersion: options.VersionString)
        {
        }
    }
}
