﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using NUnit.Framework;

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
                await WaitForCompletionAsync(await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk));
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);
                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await WaitForCompletionAsync(await DisksClient.StartDeleteAsync(rgName, diskName));
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
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
                await WaitForCompletionAsync(await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk));
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await WaitForCompletionAsync(await DisksClient.StartDeleteAsync(rgName, diskName));
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
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
                await WaitForCompletionAsync(await DisksClient.StartCreateOrUpdateAsync(rgName, diskName, disk));
                Disk diskOut = await DisksClient.GetAsync(rgName, diskName);

                Validate(disk, diskOut, disk.Location);
                Assert.AreEqual(disk.HyperVGeneration, diskOut.HyperVGeneration);
                await WaitForCompletionAsync(await DisksClient.StartDeleteAsync(rgName, diskName));
            }
            finally
            {
                await WaitForCompletionAsync(await ResourceGroupsClient.StartDeleteAsync(rgName));
            }
        }
    }
}