using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public Heartbeat(DateTime timestamp, double throughput, TimeSpan elapsed, long iterations)
        {
            Timestamp = timestamp;
            Throughput = throughput;
            Elapsed = elapsed;
            Iterations = iterations;
        }

        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public long Iterations { get; set; }

        public double Throughput { get; set; }
    }
}