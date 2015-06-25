using System.Diagnostics.CodeAnalysis;

namespace NLoad
{
    public class TestResult
    {
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