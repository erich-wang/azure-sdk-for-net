using Azure.Core.Testing;
using Azure.Identity;

using NUnit.Framework;

namespace Azure.Management.KeyVault.Tests
{
    [ClientTestFixture(KeyVaultsManagementClientOption.ServiceVersion.V2019_09_01)]
    [NonParallelizable]
    public abstract class KeyVaultsManagementClientBase : RecordedTestBase
    {
        private readonly KeyVaultsManagementClientOption.ServiceVersion _serviceVersion;

        public VaultsClient Client { get; set; }

        protected KeyVaultsManagementClientBase(bool isAsync, KeyVaultsManagementClientOption.ServiceVersion serviceVersion)
            : base(isAsync)
        {
            _serviceVersion = serviceVersion;
        }

        internal VaultsClient GetClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new VaultsClient("c9cbd920-c00c-427c-852b-8aaf38badaeb",
                recording.GetCredential(new DefaultAzureCredential()),
                recording.InstrumentClientOptions(new KeyVaultsManagementClientOption(_serviceVersion))));
        }

        public override void StartTestRecording()
        {
            base.StartTestRecording();
        }
    }
}
