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
    public class DiskRPDiskEncryptionSetTests : DiskRPTestsBase
    {
        public DiskRPDiskEncryptionSetTests(bool isAsync)
        : base(isAsync)
        {
        }
        private static string supportedZoneLocation = "southeastasia";

        [Test]
        public async Task DiskEncryptionSet_CRUD()
        {
            await DiskEncryptionSet_CRUD_Execute("DiskEncryptionSet_CRUD", location: supportedZoneLocation);
        }

        [Test]
        public async Task DiskEncryptionSet_List()
        {
            await DiskEncryptionSet_List_Execute("DiskEncryptionSet_List", location: supportedZoneLocation);
        }

        [Test]
        [Ignore("this should be tested by generate team")]
        public async Task DiskEncryptionSet_CreateDisk()
        {
            await DiskEncryptionSet_CreateDisk_Execute("DiskEncryptionSet_CreateDisk", location: supportedZoneLocation);
        }

        [Test]
        [Ignore("this should be tested by generate team")]
        public async Task DiskEncryptionSet_AddDESToExistingDisk()
        {
            await DiskEncryptionSet_UpdateDisk_Execute("DiskEncryptionSet_AddDESToExistingDisk", location: supportedZoneLocation);
        }
    }
}
