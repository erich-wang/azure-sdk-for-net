// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Azure.Management.Compute.Tests
{
    public class ComputeManagementTestUtilities : ComputeClientBase
    {
        public ComputeManagementTestUtilities(bool isAsync)
        : base(isAsync)
        {
        }

        public static string DefaultLocations = "SoutheastAsia";

        public string GenerateName(string prefix = null,
            [System.Runtime.CompilerServices.CallerMemberName]
            string methodName="GenerateName_failed")
        {
            return Recording.GetVariable(methodName, prefix);
        }
    }
}
