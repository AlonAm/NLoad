using System;

namespace NLoad
{
    public class TestRunResult
    {
        public TestRunResult()
        {
            
        }

        public TestRunResult(DateTime startTime)
        {
            StartTime = startTime;
        }

        public TestResult TestResult { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan ResponseTime
        {
            get
            {
                return EndTime - StartTime;
            }
        }
    }

    public class TestResult
    {
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