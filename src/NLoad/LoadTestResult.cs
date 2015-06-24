using System;
using System.Collections.Generic;

namespace NLoad
{
    public class LoadTestResult
    {
        public List<TestRunResult> TestRuns { get; set; }

        public long Iterations { get; set; }
        
        public TimeSpan Runtime { get; set; }
        
        public List<Heartbeat> Heartbeats { get; set; }

        public double MinThroughput { get; set; }
        public double MaxThroughput { get; set; }
        public double AverageThroughput { get; set; }

        public TimeSpan MinResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
    }
}