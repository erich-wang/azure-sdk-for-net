using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core;

[assembly: AzureResourceProviderNamespace("Microsoft.KeyVault")]

namespace Azure.Management.KeyVault
{
    public class KeyVaultsManagementClientOption: ClientOptions
    {
        private const ServiceVersion Latest = ServiceVersion.V2019_09_01;
        internal static KeyVaultsManagementClientOption Default { get; } = new KeyVaultsManagementClientOption();
        public KeyVaultsManagementClientOption(ServiceVersion serviceVersion = Latest)
        {
            VersionString = serviceVersion switch
            {
                ServiceVersion.V2019_09_01 => "2019-09-01",
                _ => throw new ArgumentOutOfRangeException(nameof(serviceVersion))
            };
        }
        internal string VersionString { get; }
        public enum ServiceVersion
        {
#pragma warning disable CA1707
            V2019_09_01 = 1
#pragma warning restore CA1707
        }
    }
}
