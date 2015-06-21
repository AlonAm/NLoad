using System;

namespace NLoad
{
    public class LoadTestResult
    {
        public long TotalTestRuns { get; set; }
        
        public TimeSpan TotalRuntime { get; set; }
    }
}