using System;

namespace NLoad
{
    public class HeartbeatEventArgs : EventArgs
    {
        public double Throughput { get; set; }
    }
}