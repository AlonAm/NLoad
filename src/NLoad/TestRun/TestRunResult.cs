using System;

namespace NLoad
{
    public class TestRunResult
    {
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
}