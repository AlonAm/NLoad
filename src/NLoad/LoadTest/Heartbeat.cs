﻿using System;

namespace NLoad
{
    public class Heartbeat : EventArgs
    {
        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public long TotalIterations { get; set; }

        public long TotalErrors { get; set; }

        public double Throughput { get; set; }

        public long TotalThreads { get; set; }
    }
}