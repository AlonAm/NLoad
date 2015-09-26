using System;

namespace NLoad
{
    /// <summary>
    /// The load test configuration and settings
    /// </summary>
    public class LoadTestConfiguration
    {
        public LoadTestConfiguration()
        {
            Defaults();
        }

        private void Defaults()
        {
            NumberOfThreads = 1;
            Duration = TimeSpan.Zero;
            DelayBetweenThreadStart = TimeSpan.Zero;
            StartImmediately = false;
        }

        /// <summary>
        /// The test scenarion type
        /// </summary>
        public Type TestType { get; set; }

        /// <summary>
        /// The number of threads that will be used during the load test.
        /// </summary>
        public int NumberOfThreads { get; set; }

        /// <summary>
        /// The load test run duration.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The time delay between threads.
        /// </summary>
        public TimeSpan DelayBetweenThreadStart { get; set; }

        /// <summary>
        /// Start load test immediately or wait until threads created.
        /// </summary>
        public bool StartImmediately { get; set; }
    }
}