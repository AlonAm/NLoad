using System;

namespace NLoad
{
    public class TestRunResult
    {
        public TestRunResult(DateTime startTime)
        {
            StartTime = startTime;
        }

        public TestResult TestResult { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }
    }

    public class TestResult
    {
        public bool Passed { get; set; }

        public string ErrorMessage { get; set; }
    }
}