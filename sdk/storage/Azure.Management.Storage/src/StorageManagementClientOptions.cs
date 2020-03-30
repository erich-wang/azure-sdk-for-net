// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure.Core;

namespace Azure.Management.Storage
{
    /// <summary>
    /// The options for <see cref="StorageManagementClientOptions"/>
    /// </summary>
    public class StorageManagementClientOptions : ClientOptions
    {
        private const ServiceVersion Latest = ServiceVersion.V2019_06_01;
        internal static StorageManagementClientOptions Default { get; } = new StorageManagementClientOptions();

        /// <summary>
        /// StorageManagementClientOptions
        /// </summary>
        /// <param name="serviceVersion"></param>
        public StorageManagementClientOptions(ServiceVersion serviceVersion = Latest)
        {
            VersionString = serviceVersion switch
            {
                ServiceVersion.V2019_06_01 => "2019-06-01",
                _ => throw new ArgumentOutOfRangeException(nameof(serviceVersion))
            };
        }
        internal string VersionString { get; }

        /// <summary>
        /// The template service version.
        /// </summary>
        public enum ServiceVersion
        {
#pragma warning disable CA1707
            /// <summary>
            /// V2019_06_01
            /// </summary>
            V2019_06_01 = 1
#pragma warning restore CA1707
        }
    }
}
