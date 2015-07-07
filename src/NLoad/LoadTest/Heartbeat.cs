using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public Heartbeat(DateTime timestamp, double throughput, TimeSpan elapsed, long totalIterations, long totalErrors)
        {
            Timestamp = timestamp;
            Throughput = throughput;
            Elapsed = elapsed;
            TotalIterations = totalIterations;
            TotalErrors = totalErrors;
        }

        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public long TotalIterations { get; set; }

        public long TotalErrors { get; set; }

        public double Throughput { get; set; }

        [Obsolete("not in use")]
        public int ThreadCount { get; set; }
    }
}