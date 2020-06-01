// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class HelpersTests :ComputeClientBase
    {
        public HelpersTests(bool isAsync)
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
        public void TestUtilityFunctions()
        {
            Assert.AreEqual("Azure.Management.Compute.Tests.HelpersTests", this.GetType().FullName);
#if NET46
            Assert.Equal("TestUtilityFunctions", TestUtilities.GetCurrentMethodName(1));
#else
            //Assert.AreEqual("TestUtilityFunctions", TestUtilities.GetCurrentMethodName());
#endif
        }
    }
}
