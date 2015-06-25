using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public Heartbeat(DateTime timestamp, double throughput, TimeSpan elapsed)
        {
            Timestamp = timestamp;
            Throughput = throughput;
            Elapsed = elapsed;
        }

        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public double Throughput { get; set; }
    }
}