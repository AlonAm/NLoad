namespace NLoad
{
    public class TestResult
    {
        public static readonly TestResult Success = new TestResult();
        public static readonly TestResult Failed = new TestResult(false);

        public TestResult(bool passed = true, string errorMessage = "")
        {
            Passed = passed;
            ErrorMessage = errorMessage;
        }

        public bool Passed { get; private set; }

        public string ErrorMessage { get; private set; }
    }
}