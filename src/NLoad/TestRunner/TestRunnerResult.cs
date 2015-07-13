using System;
using System.Collections.Generic;

namespace NLoad
{
    public class TestRunnerResult
    {
        public TestRunnerResult()
        {
        }

        public TestRunnerResult(DateTime starTime)
        {
            StartTime = starTime;
        }

        public long Iterations { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<TestRunResult> TestRuns { get; set; }
    }
}