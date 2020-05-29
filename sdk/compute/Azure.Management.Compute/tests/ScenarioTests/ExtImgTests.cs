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

namespace Azure.Management.Compute.Tests
{
    public class ExtImgTests:ComputeClientBase
    {
        public ExtImgTests(bool isAsync)
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

        private class VirtualMachineExtensionImageGetParameters
        {
            public string Location = "westus";
            public string PublisherName = "Microsoft.Compute";
            public string Type = "VMAccessAgent";
            public string FilterExpression = "";
        }

        private static readonly string existingVersion = "2.4.5";

        private static readonly VirtualMachineExtensionImageGetParameters parameters =
            new VirtualMachineExtensionImageGetParameters();

        [Test]
        public async Task TestExtImgGet()
        {
            var vmimageext = await VirtualMachineExtensionImagesClient.GetAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type,
                existingVersion);

            Assert.True(vmimageext.Value.Name == existingVersion);
            Assert.True(vmimageext.Value.Location == "westus");

            Assert.True(vmimageext.Value.OperatingSystem == "Windows");
            Assert.True(vmimageext.Value.ComputeRole == "IaaS");
            Assert.True(vmimageext.Value.HandlerSchema == null);
            Assert.True(vmimageext.Value.VmScaleSetEnabled == false);
            Assert.True(vmimageext.Value.SupportsMultipleExtensions == false);
        }

        [Test]
        public async Task TestExtImgListTypes()
        {
            var vmextimg =await VirtualMachineExtensionImagesClient.ListTypesAsync(
                parameters.Location,
                parameters.PublisherName);
            Assert.True(vmextimg.Value.Count > 0);
            Assert.True(vmextimg.Value.Count(vmi => vmi.Name == "VMAccessAgent") != 0);
        }

        [Test]
        public async Task TestExtImgListVersionsNoFilter()
        {
            var vmextimg = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type);

            Assert.True(vmextimg.Value.Count > 0);
            Assert.True(vmextimg.Value.Count(vmi => vmi.Name == existingVersion) != 0);
        }

        [Test]
        public async Task TestExtImgListVersionsFilters()
        {
            string existingVersionPrefix = existingVersion.Substring(0, existingVersion.LastIndexOf('.'));

            // Filter: startswith - Positive Test
            parameters.FilterExpression = null;
            var extImages = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type);
            Assert.True(extImages.Value.Count > 0);

            string ver = extImages.Value.First().Name;
            //var query = new Microsoft.Rest.Azure.OData.ODataQuery<Azure.Management.Compute.Models.VirtualMachineExtensionImage>();
            string query = "startswith(name,'" + ver + "')";
            //parameters.FilterExpression = "$filter=startswith(name,'" + ver + "')";
            var vmextimg = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type,
                query);
            Assert.True(vmextimg.Value.Count > 0);
            Assert.True(vmextimg.Value.Count(vmi => vmi.Name != existingVersionPrefix) != 0);

            // Filter: startswith - Negative Test
            query = "startswith(name,'" + existingVersionPrefix + "')";
            //query.SetFilter(f => f.Name.StartsWith(existingVersionPrefix));
            parameters.FilterExpression = string.Format("$filter=startswith(name,'{0}')", existingVersionPrefix);
            vmextimg = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type,
                query);
            Assert.True(vmextimg.Value.Count > 0);
            Assert.True(vmextimg.Value.Count(vmi => vmi.Name == existingVersionPrefix) == 0);

            // Filter: top - Positive Test
            //query.Filter = null;
            //query.Top = 1;
            parameters.FilterExpression = "$top=1";
            vmextimg = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type,
                null,
                1);
            Assert.True(vmextimg.Value.Count == 1);
            Assert.True(vmextimg.Value.Count(vmi => vmi.Name == existingVersion) != 0);

            // Filter: top - Negative Test
            //query.Replace("Top eq 1", "Top eq 0");
            //query.Top = 0;
            parameters.FilterExpression = "$top=0";
            vmextimg = await VirtualMachineExtensionImagesClient.ListVersionsAsync(
                parameters.Location,
                parameters.PublisherName,
                parameters.Type,
                null,
                0);
            Assert.True(vmextimg.Value.Count == 0);
        }
    }
}
