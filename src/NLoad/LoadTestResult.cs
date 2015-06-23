using System;
using System.Collections.Generic;

namespace NLoad
{
    public class LoadTestResult
    {
        public List<TestResult> TestsResults { get; set; }

        public long Iterations { get; set; }
        
        public TimeSpan Runtime { get; set; }
        
        public List<HeartbeatEventArgs> Heartbeats { get; set; }
    }
}