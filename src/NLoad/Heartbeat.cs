using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public Heartbeat(DateTime timestamp, double throughput)
        {
            Timestamp = timestamp;
            Throughput = throughput;
        }

        public DateTime Timestamp { get; set; }

        public double Throughput { get; set; }
    }
}