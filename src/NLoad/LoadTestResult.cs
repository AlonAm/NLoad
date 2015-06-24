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
    }
}