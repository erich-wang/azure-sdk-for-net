// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using System.Reflection;
using System.Threading;
using Azure.Management.Compute.Tests;
using Azure.Management.Compute;
using Azure.Management.Compute.Models;
using System.Text.Json;
using System.Net;

namespace Azure.Management.Compute.Tests.DiskRPTests
{
    public class DiskRPCreateOptionTests: DiskRPTestsBase
    {
        public DiskRPCreateOptionTests(bool isAsync)
        : base(isAsync)
        {
        }
        private static string DiskRPLocation = "centralus";

        /// <summary>
        /// positive test for testing upload disks
        /// </summary>
        [Test]
        public async Task UploadDiskPositiveTest()
        {
            EnsureClientsInitialized();

            var rgName = Recording.GenerateAssetName(TestPrefix);
            var diskName = Recording.GenerateAssetName(DiskNamePrefix);
            Disk disk = await GenerateDefaultDisk(DiskCreateOption.Upload.ToString(), rgName, 32767);
            disk.Location = DiskRPLocation;
            try
            {
                await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(DiskRPLocation));
                //put disk
                await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk);
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.CreationData.CreateOption, diskOut.CreationData.CreateOption);
                await DisksClient.StartDeleteAsync(rgName, diskName);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        /// <summary>
        /// positive test for testing disks created from a gallery image version
        /// </summary>
        [Test]
        [Ignore("this should be tested by generate team")]
        public async Task DiskFromGalleryImageVersion()
        {
            EnsureClientsInitialized();

            var rgName = Recording.GenerateAssetName(TestPrefix);
            var diskName = Recording.GenerateAssetName(DiskNamePrefix);
            Disk disk = GenerateBaseDisk(DiskCreateOption.FromImage.ToString());
            disk.Location = DiskRPLocation;
            disk.CreationData.GalleryImageReference = new ImageDiskReference("/subscriptions/0296790d-427c-48ca-b204-8b729bbd8670/resourceGroups/swaggertests/providers/Microsoft.Compute/galleries/swaggergallery/images/lunexample2/versions/1.0.0", 1);
            try
            {
                await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(DiskRPLocation));
                //put disk
                await (await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk)).WaitForCompletionAsync();
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.CreationData.CreateOption, diskOut.CreationData.CreateOption);
                await (await DisksClient.StartDeleteAsync(rgName, diskName)).WaitForCompletionAsync();
            }
            finally
            {
               await (await ResourceGroupsClient.StartDeleteAsync(rgName)).WaitForCompletionAsync();
            }
        }
    }
}
