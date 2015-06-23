using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public double Throughput { get; set; }
    }
}