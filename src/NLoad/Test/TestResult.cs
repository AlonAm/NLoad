using System;
using System.Diagnostics.CodeAnalysis;

namespace NLoad
{
    public class TestResult
    {
        [Obsolete("Use TestResult.Success instead.")]
        public static readonly TestResult Default = new TestResult(true);

        public static readonly TestResult Success = new TestResult(true);
        public static readonly TestResult Failed = new TestResult(true);

        [ExcludeFromCodeCoverage]
        public TestResult()
        {
        }

        public TestResult(bool passed)
        {
            Passed = passed;
        }

        public bool Passed { get; set; }

        public string ErrorMessage { get; set; }
    }
}