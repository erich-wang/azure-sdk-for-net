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
//using Newtonsoft.Json;
using Castle.Core.Logging;

namespace Azure.Management.Compute.Tests
{
    public class ComputeManagementTestUtilities : ComputeClientBase
    {
        public ComputeManagementTestUtilities(bool isAsync)
        : base(isAsync)
        {
        }
        public static string DefaultLocations = "SoutheastAsia";
        //public void WaitSeconds(double seconds)
        //{
        //    if (Mode == RecordedTestMode.Playback)
        //    {
        //        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
        //    }
        //}

        //public void WaitMinutes(double minutes)
        //{
        //    WaitSeconds(minutes * 60);
        //}

        public string GenerateName(string prefix = null,
            [System.Runtime.CompilerServices.CallerMemberName]
            string methodName="GenerateName_failed")
        {
            return Recording.GetVariable(methodName, prefix);
        }
    }
}
