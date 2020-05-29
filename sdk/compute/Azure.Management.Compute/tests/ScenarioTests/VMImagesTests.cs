﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Azure.Management.Compute.Tests
{
    public class VMImagesTests:ComputeClientBase
    {
        public VMImagesTests(bool isAsync)
         : base(isAsync)
        {
        }
        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                InitializeBase();
            }
            //ComputeManagementClient computeClient;
            //ResourceManagementClient resourcesClient;
        }
        [Test]
        public async Task TestVMImageGet()
        {
            string[] availableWindowsServerImageVersions = (await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter")).Value.Select(t => t.Name).ToArray();

            var vmimage = await VirtualMachineImagesClient.GetAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                availableWindowsServerImageVersions[0]);

            Assert.AreEqual(availableWindowsServerImageVersions[0], vmimage.Value.Name);
            //Assert.AreEqual("southeastasia", vmimage.Value.Location);
            Assert.AreEqual(DefaultLocation, vmimage.Value.Location);

            // FIXME: This doesn't work with a real Windows Server images, which is what's in the query parameters.
            // Bug 4196378
            /*
            Assert.True(vmimage.VirtualMachineImage.PurchasePlan.Name == "name");
            Assert.True(vmimage.VirtualMachineImage.PurchasePlan.Publisher == "publisher");
            Assert.True(vmimage.VirtualMachineImage.PurchasePlan.Product == "product");
            */

            Assert.AreEqual(OperatingSystemTypes.Windows, vmimage.Value.OsDiskImage.OperatingSystem);

            //Assert.True(vmimage.VirtualMachineImage.DataDiskImages.Count(ddi => ddi.Lun == 123456789) != 0);
        }

        [Test]
        public async Task TestVMImageAutomaticOSUpgradeProperties()
        {
            // Validate if images supporting automatic OS upgrades return
            // AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported = true in GET VMImageVesion call
            string imagePublisher = "MicrosoftWindowsServer";
            string imageOffer = "WindowsServer";
            string imageSku = "2016-Datacenter";
            string[] availableWindowsServerImageVersions = (await VirtualMachineImagesClient.ListAsync(
                DefaultLocation, imagePublisher, imageOffer, imageSku)).Value.Select(t => t.Name).ToArray();

            string firstVersion = availableWindowsServerImageVersions.First();
            string lastVersion = null;
            if (availableWindowsServerImageVersions.Length >= 2)
            {
                lastVersion = availableWindowsServerImageVersions.Last();
            }

            var vmimage = await VirtualMachineImagesClient.GetAsync(
                DefaultLocation, imagePublisher, imageOffer, imageSku, firstVersion);
            Assert.True(vmimage.Value.AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported);

            if (!string.IsNullOrEmpty(lastVersion))
            {
                vmimage = await VirtualMachineImagesClient.GetAsync(
                    DefaultLocation, imagePublisher, imageOffer, imageSku, lastVersion);
                Assert.True(vmimage.Value.AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported);
            }

            // Validate if image not whitelisted to support automatic OS upgrades, return
            // AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported = false in GET VMImageVesion call
            imagePublisher = "Canonical";
            imageOffer = "UbuntuServer";
            imageSku = (await VirtualMachineImagesClient.ListSkusAsync(DefaultLocation, imagePublisher, imageOffer)).Value.FirstOrDefault().Name;
            string[] availableUbuntuImageVersions = (await VirtualMachineImagesClient.ListAsync(
                DefaultLocation, imagePublisher, imageOffer, imageSku)).Value.Select(t => t.Name).ToArray();

            firstVersion = availableUbuntuImageVersions.First();
            lastVersion = null;
            if (availableUbuntuImageVersions.Length >= 2)
            {
                lastVersion = availableUbuntuImageVersions.Last();
            }

            vmimage = await VirtualMachineImagesClient.GetAsync(
                DefaultLocation, imagePublisher, imageOffer, imageSku, firstVersion);
            Assert.False(vmimage.Value.AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported);

            if (!string.IsNullOrEmpty(lastVersion))
            {
                vmimage = await VirtualMachineImagesClient.GetAsync(
                    DefaultLocation, imagePublisher, imageOffer, imageSku, lastVersion);
                Assert.False(vmimage.Value.AutomaticOSUpgradeProperties.AutomaticOSUpgradeSupported);
            }
        }

        [Test]
        public async Task TestVMImageListNoFilter()
        {
            var vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter");

            Assert.True(vmimages.Value.Count > 0);
            //Assert.True(vmimages.Count(vmi => vmi.Name == AvailableWindowsServerImageVersions[0]) != 0);
            //Assert.True(vmimages.Count(vmi => vmi.Name == AvailableWindowsServerImageVersions[1]) != 0);
        }

        [Test]
        public async Task TestVMImageListFilters()
        {
            // Filter: top - Negative Test
            var vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 0);
            Assert.True(vmimages.Value.Count == 0);

            // Filter: top - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 1);
            Assert.True(vmimages.Value.Count == 1);

            // Filter: top - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 2);
            Assert.True(vmimages.Value.Count == 2);

            // Filter: orderby - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                orderby: "name desc");

            // Filter: orderby - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 2,
                orderby: "name asc");
            Assert.True(vmimages.Value.Count == 2);

            // Filter: top orderby - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 1,
                orderby: "name desc");
            Assert.True(vmimages.Value.Count == 1);

            // Filter: top orderby - Positive Test
            vmimages = await VirtualMachineImagesClient.ListAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer",
                "2012-R2-Datacenter",
                top: 1,
                orderby: "name asc");
            Assert.True(vmimages.Value.Count == 1);
        }

        [Test]
        public async Task TestVMImageListPublishers()
        {
            var publishers = await VirtualMachineImagesClient.ListPublishersAsync(
                DefaultLocation);

            Assert.True(publishers.Value.Count > 0);
            Assert.True(publishers.Value.Count(pub => pub.Name == "MicrosoftWindowsServer") != 0);
        }

        [Test]
        public async Task TestVMImageListOffers()
        {
            var offers = await VirtualMachineImagesClient.ListOffersAsync(
                DefaultLocation,
                "MicrosoftWindowsServer");

            Assert.True(offers.Value.Count > 0);
            Assert.True(offers.Value.Count(offer => offer.Name == "WindowsServer") != 0);
        }

        [Test]
        public async Task TestVMImageListSkus()
        {
            var skus = await VirtualMachineImagesClient.ListSkusAsync(
                DefaultLocation,
                "MicrosoftWindowsServer",
                "WindowsServer");

            Assert.True(skus.Value.Count > 0);
            Assert.True(skus.Value.Count(sku => sku.Name == "2012-R2-Datacenter") != 0);
        }
    }
}
