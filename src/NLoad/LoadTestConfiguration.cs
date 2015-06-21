using System;

namespace NLoad
{
    public class LoadTestConfiguration
    {
        public int NumberOfThreads { get; set; }
        
        public TimeSpan Duration { get; set; }
        
        public TimeSpan DelayBetweenThreadStart { get; set; }
    }
}