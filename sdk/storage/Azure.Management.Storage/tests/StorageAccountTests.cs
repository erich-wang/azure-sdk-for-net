// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure;
using NUnit.Framework;

namespace StorageAccounts.Tests
{
    public class StorageAccountTests
    {
        [Fact]
        public void StorageAccountCreateTest()
        {
            var handler = new RecordedDelegatingHandler { StatusCodeToReturn = HttpStatusCode.OK };

            using (MockContext context = MockContext.Start(this.GetType()))
            {
                var resourcesClient = StorageManagementTestUtilities.GetResourceManagementClient(context, handler);
                var storageMgmtClient = StorageManagementTestUtilities.GetStorageManagementClient(context, handler);

                // Create resource group
                var rgname = StorageManagementTestUtilities.CreateResourceGroup(resourcesClient);

                // Create storage account
                string accountName = TestUtilities.GenerateName("sto");
                var parameters = StorageManagementTestUtilities.GetDefaultStorageAccountParameters();
                var account = storageMgmtClient.StorageAccounts.Create(rgname, accountName, parameters);
                StorageManagementTestUtilities.VerifyAccountProperties(account, true);

                // Make sure a second create returns immediately
                var createRequest = storageMgmtClient.StorageAccounts.Create(rgname, accountName, parameters);
                StorageManagementTestUtilities.VerifyAccountProperties(account, true);

                // Create storage account with only required params
                accountName = TestUtilities.GenerateName("sto");
                parameters = new StorageAccountCreateParameters
                {
                    Sku = new Sku { Name = StorageManagementTestUtilities.DefaultSkuName },
                    Kind = Kind.Storage,
                    Location = StorageManagementTestUtilities.DefaultLocation,
                };
                storageMgmtClient.StorageAccounts.Create(rgname, accountName, parameters);
                StorageManagementTestUtilities.VerifyAccountProperties(account, false);
            }
        }
    }
}
