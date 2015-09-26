using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad
{
    /// <summary>
    /// Load Test Heart Rate Monitor
    ///  </summary>
    public class HeartRateMonitor
    {
        private readonly ILoadTest _loadTest;

        public event EventHandler<Heartbeat> Heartbeat;

        public HeartRateMonitor(ILoadTest loadTest)
        {
            if (loadTest == null)
            {
                throw new ArgumentNullException("loadTest");
            }

            _loadTest = loadTest;
        }

        public CancellationToken CancellationToken { get; set; }

        public void Start(DateTime startTime, TimeSpan duration)
        {
            var running = true;

            var start = DateTime.UtcNow;

            Heartbeats = new List<Heartbeat>();

            while (running)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var now = DateTime.UtcNow;

                var runtime = now - start;

                var iterations = _loadTest.TotalIterations;

                var throughput = iterations / runtime.TotalSeconds;

                if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue;

                var heartbeat = new Heartbeat
                {
                    Timestamp = now,
                    Runtime = runtime,
                    TotalRuntime = DateTime.Now - startTime,
                    Throughput = throughput,
                    TotalIterations = iterations,
                    TotalErrors = _loadTest.TotalErrors,
                    TotalThreads = _loadTest.TotalThreads
                };

                Heartbeats.Add(heartbeat);

                OnHeartbeat(heartbeat);

                if (runtime >= duration)
                {
                    running = false;
                }
                else if (DateTime.UtcNow - start < duration)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            var handler = Heartbeat;

            if (handler != null)
            {
                handler(this, heartbeat); //todo: add try catch?
            }
        }

        public List<Heartbeat> Heartbeats { get; set; }
    }
}
