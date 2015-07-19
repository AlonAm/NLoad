using System;

namespace NLoad
{
    public class LoadTestConfiguration
    {
        public LoadTestConfiguration()
        {
            NumberOfThreads = 1;
            Duration = TimeSpan.Zero;
            DelayBetweenThreadStart = TimeSpan.Zero;
        }

        public Type TestType { get; set; }

        public int NumberOfThreads { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan DelayBetweenThreadStart { get; set; }
    }
}