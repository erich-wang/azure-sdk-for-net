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
    public class ListSkuTests:ComputeClientBase
    {
        public ListSkuTests(bool isAsync)
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
        public async Task TestListSkus()
        {
            var skus = ResourceSkusClient.ListAsync();
            var skusres = await skus.ToEnumerableAsync();
            Assert.True(skusres.Any(), "Assert that the array of skus has at least 1 member.");
            Assert.True(skusres.Any(sku => sku.ResourceType == "availabilitySets"), "Assert that the sku list at least contains" +
                                                                                 "one availability set.");
            Assert.True(skusres.Any(sku => sku.ResourceType == "virtualMachines"), "Assert that the sku list at least contains" +
                                                                                "one virtual machine.");
            Assert.True(skusres.Any(sku => sku.LocationInfo != null), "Assert that the sku list has non null location info in it.");
            Assert.True(skusres.All(sku => sku.LocationInfo.Count == 1), "There should be exactly one location info per entry.");
            Assert.True(skusres.Any(sku => sku.LocationInfo[0].Location.Equals("westus", StringComparison.Ordinal)), "Assert that it has entry for one of the CRP regions (randomly picked).");

            // EastUS2EUAP is one of the regions where UltraSSD is enabled, hence verifying that CRP and ARM are returning correct
            // properties related to UltraSSD in the SKUs API
            var vmSkusInEastUS2Euap = skusres.Where(
                s => s.Locations[0].Equals("eastus2euap", StringComparison.OrdinalIgnoreCase) && s.ResourceType == "virtualMachines").ToList();

            var ultraSSDSupportingSku = vmSkusInEastUS2Euap.First(s => s.Name == "Standard_B2s");
            var nonUltraSSDSupportingSku = vmSkusInEastUS2Euap.First(s => s.Name == "Standard_A7");

            Assert.NotNull(ultraSSDSupportingSku.LocationInfo);
            Assert.AreEqual(1, ultraSSDSupportingSku.LocationInfo.Count);
            Assert.NotNull(ultraSSDSupportingSku.LocationInfo[0].ZoneDetails);
            Assert.AreEqual(1, ultraSSDSupportingSku.LocationInfo[0].ZoneDetails.Count);
            Assert.NotNull(ultraSSDSupportingSku.LocationInfo[0].ZoneDetails[0].Name);
            Assert.NotNull(ultraSSDSupportingSku.LocationInfo[0].ZoneDetails[0].Capabilities);
            Assert.AreEqual(1, ultraSSDSupportingSku.LocationInfo[0].ZoneDetails[0].Capabilities.Count);
            Assert.AreEqual("UltraSSDAvailable", ultraSSDSupportingSku.LocationInfo[0].ZoneDetails[0].Capabilities[0].Name);
            Assert.AreEqual("True", ultraSSDSupportingSku.LocationInfo[0].ZoneDetails[0].Capabilities[0].Value);

            Assert.NotNull(nonUltraSSDSupportingSku.LocationInfo);
            // ZoneDetails should not be set for a SKU which does not support ultraSSD. This is because we do not have any
            // other zonal capability currently.
            Assert.Null(nonUltraSSDSupportingSku.LocationInfo[0].ZoneDetails);
        }
    }
}
