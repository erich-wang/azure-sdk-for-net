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
    public class DiskRPHyperVGenerationTests : DiskRPTestsBase
    {
        public DiskRPHyperVGenerationTests(bool isAsync)
        : base(isAsync)
        {
        }
        private static string DiskRPLocation = "westcentralus";

        [Test]
        public async Task DiskHyperVGeneration1PositiveTest()
        {
            EnsureClientsInitialized();

            var rgName = Recording.GenerateAssetName(TestPrefix);
            var diskName = Recording.GenerateAssetName(DiskNamePrefix);
            Disk disk = await GenerateDefaultDisk(DiskCreateOption.Empty.ToString(), rgName, 10);
            disk.HyperVGeneration = HyperVGeneration.V1;
            disk.Location = DiskRPLocation;

            try
            {
                await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(DiskRPLocation));
                //put disk
                await (await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk)).WaitForCompletionAsync();
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await (await DisksClient.StartDeleteAsync(rgName, diskName)).WaitForCompletionAsync();
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        [Test]
        public async Task DiskHyperVGeneration2PositiveTest()
        {
            EnsureClientsInitialized();

            var rgName = Recording.GenerateAssetName(TestPrefix);
            var diskName = Recording.GenerateAssetName(DiskNamePrefix);
            Disk disk = await GenerateDefaultDisk(DiskCreateOption.Empty.ToString(), rgName, 10);
            disk.HyperVGeneration = HyperVGeneration.V2;
            disk.Location = DiskRPLocation;

            try
            {
                await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(DiskRPLocation));
                //put disk
                await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk);
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await DisksClient.StartDeleteAsync(rgName, diskName);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }

        [Test]
        public async Task DiskHyperVGenerationOmittedTest()
        {
            EnsureClientsInitialized();

            var rgName = Recording.GenerateAssetName(TestPrefix);
            var diskName = Recording.GenerateAssetName(DiskNamePrefix);
            Disk disk = await GenerateDefaultDisk(DiskCreateOption.Empty.ToString(), rgName, 10);
            disk.Location = DiskRPLocation;

            try
            {
                await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup (DiskRPLocation ));
                //put disk
                await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk);
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await DisksClient.StartDeleteAsync(rgName, diskName);
            }
            finally
            {
                await ResourceGroupsClient.StartDeleteAsync(rgName);
            }
        }
    }
}
