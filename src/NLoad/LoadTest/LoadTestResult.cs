using System;
using System.Collections.Generic;

namespace NLoad
{
    public class LoadTestResult
    {
        public IEnumerable<LoadGeneratorResult> TestRunnersResults { get; set; }

        public List<TestRunResult> TestRuns { get; set; }

        public List<Heartbeat> Heartbeat { get; set; }

        public long TotalIterations { get; set; }
        public long TotalErrors { get; set; }

        public TimeSpan TotalRuntime { get; set; }

        public double MinThroughput { get; set; }
        public double MaxThroughput { get; set; }
        public double AverageThroughput { get; set; }

        public TimeSpan MinResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        
    }
}