using Azure.Core;
using Azure.Core.Pipeline;

namespace Azure.Management.KeyVault
{
    public partial class VaultsClient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "AZC0007:DO provide a minimal constructor that takes only the parameters required to connect to the service.", Justification = "<Pending>")]
        public VaultsClient(string subscriptionId, TokenCredential tokenCredential) : this(subscriptionId, tokenCredential, KeyVaultsManagementClientOption.Default)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "AZC0007:DO provide a minimal constructor that takes only the parameters required to connect to the service.", Justification = "<Pending>")]
        public VaultsClient(string subscriptionId, TokenCredential tokenCredential, KeyVaultsManagementClientOption options) :
            this(new ClientDiagnostics(options), ManagementClientPipeline.Build(options, tokenCredential), subscriptionId, apiVersion: options.VersionString)
        {
        }
    }
}
