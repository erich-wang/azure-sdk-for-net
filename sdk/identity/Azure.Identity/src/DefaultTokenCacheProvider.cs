﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Azure.Identity
{
    /// <summary>
    /// Persists the token cache to the default shared token cache location utilizing the user data protection mechanisms available on the current platform.
    /// </summary>
    public sealed class DefaultTokenCacheProvider : TokenCacheProvider
    {
        internal bool AllowUnencryptedCache { get; }

        private static readonly DefaultTokenCacheProvider s_protected = new DefaultTokenCacheProvider(false);
        private static readonly DefaultTokenCacheProvider s_fallback = new DefaultTokenCacheProvider(true);

        /// <summary>
        /// Persists the token cache to the default shared token cache location utilizing the user data protection mechanisms available on the current platform. If no data protection
        /// mechanism is available an exception will be raised when the credential is used.
        /// </summary>
        public static DefaultTokenCacheProvider Protected { get { return s_protected; } }

        /// <summary>
        /// Persists the token cache to the default shared token cache location utilizing the user data protection mechanisms available on the current platform. If no data protection
        /// mechanism is available the provider will fall back to persisting the cache to an unencrypted file.
        /// </summary>
        public static DefaultTokenCacheProvider WithUnencryptedFallback { get { return s_fallback; } }

        /// <summary>
        /// Creates a new <see cref="DefaultTokenCacheProvider"/>.
        /// </summary>
        /// <param name="allowUnencryptedCache">If true allows the token cache to be persisted to the disk without encryption on systems not supporting user data protection.</param>
        private DefaultTokenCacheProvider(bool allowUnencryptedCache)
        {
            AllowUnencryptedCache = allowUnencryptedCache;
        }

        /// <summary>
        /// Reads serialized token cache data from it's persisted state.
        /// </summary>
        /// <returns>Returns the serialized token cache data.</returns>
        public override Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes serialized token cache data to it's persisted state.
        /// </summary>
        /// <param name="bytes">The serialized token cache data to be persisted.</param>
        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        internal override async Task RegisterCache(ITokenCache tokenCache)
        {
            MsalCacheHelper cacheHelper;

            StorageCreationProperties storageProperties = new StorageCreationPropertiesBuilder(Constants.DefaultMsalTokenCacheName, Constants.DefaultMsalTokenCacheDirectory, "1950a258-227b-4e31-a9cf-717495945fc2")
                .WithMacKeyChain(Constants.DefaultMsalTokenCacheKeychainService, Constants.DefaultMsalTokenCacheKeychainAccount)
                .WithLinuxKeyring(Constants.DefaultMsalTokenCacheKeyringSchema, Constants.DefaultMsalTokenCacheKeyringCollection, Constants.DefaultMsalTokenCacheKeyringLabel, Constants.DefaultMsaltokenCacheKeyringAttribute1, Constants.DefaultMsaltokenCacheKeyringAttribute2)
                .Build();

            try
            {
                cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties).ConfigureAwait(false);

                cacheHelper.VerifyPersistence();
            }
            catch (MsalCachePersistenceException)
            {
                if (AllowUnencryptedCache)
                {
                    storageProperties = new StorageCreationPropertiesBuilder(Constants.DefaultMsalTokenCacheName, Constants.DefaultMsalTokenCacheDirectory, "1950a258-227b-4e31-a9cf-717495945fc2")
                        .WithMacKeyChain(Constants.DefaultMsalTokenCacheKeychainService, Constants.DefaultMsalTokenCacheKeychainAccount)
                        .WithLinuxUnprotectedFile()
                        .Build();

                    cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties).ConfigureAwait(false);

                    cacheHelper.VerifyPersistence();
                }
                else
                {
                    throw;
                }
            }

            cacheHelper.RegisterCache(tokenCache);
        }
    }
}
