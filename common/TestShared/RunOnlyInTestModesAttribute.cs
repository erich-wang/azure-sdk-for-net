// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Azure.Core.TestFramework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class RunOnlyInTestModesAttribute : NUnitAttribute, IApplyToTest
    {
        public bool RecordOrPlayback { get; set; }
        public bool Live { get; set; }

        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable)
            {
                RecordedTestMode mode = RecordedTestUtilities.GetModeFromEnvironment();
                if (!CanRunForTestModes(mode))
                {
                    test.RunState = RunState.Ignored;
                    test.Properties.Set(PropertyNames.SkipReason, $"This test won't be run when AZURE_TEST_MODE is {mode}.");
                }
            }
        }

        private bool CanRunForTestModes(RecordedTestMode mode)
        {
            return Live && mode == RecordedTestMode.Live ||
                RecordOrPlayback && (mode == RecordedTestMode.Playback || mode == RecordedTestMode.Record);
        }
    }
}
