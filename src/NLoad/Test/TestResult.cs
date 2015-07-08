namespace NLoad
{
    public class TestResult
    {
        public static readonly TestResult Success = new TestResult();
        public static readonly TestResult Failure = new TestResult(false);

        public TestResult(bool passed = true, string errorMessage = "")
        {
            Passed = passed;
            ErrorMessage = errorMessage;
        }

        public bool Passed { get; private set; }
        
        public bool Failed { get { return !Passed; } }

        public string ErrorMessage { get; private set; }
    }
}