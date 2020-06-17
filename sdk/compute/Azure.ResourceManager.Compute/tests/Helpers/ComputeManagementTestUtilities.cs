// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Azure.ResourceManager.Compute.Tests
{
<<<<<<< HEAD
    public class ComputeManagementTestUtilities : ComputeClientBase
    {
        public ComputeManagementTestUtilities(bool isAsync)
        : base(isAsync)
        {
        }

        public static string DefaultLocations = "southeastasia";

        public string GenerateName(string prefix = null,
            [System.Runtime.CompilerServices.CallerMemberName]
            string methodName="GenerateName_failed")
        {
            return Recording.GetVariable(methodName, prefix);
        }
=======
    public class ComputeManagementTestUtilities
    {
        public static string DefaultLocations = "southeastasia";
>>>>>>> erichmaster/track2/compute.tests
    }
}
