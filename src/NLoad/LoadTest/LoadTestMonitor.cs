using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad
{
    public class LoadTestMonitor
    {
        private readonly ILoadTest _loadTest;

        public event EventHandler<Heartbeat> Heartbeat;

        public LoadTestMonitor(ILoadTest loadTest)
        {
            if (loadTest == null)
                throw new ArgumentNullException("loadTest");

            _loadTest = loadTest;
        }

        public CancellationToken CancellationToken { get; set; }

        public List<Heartbeat> Start(DateTime startTime, TimeSpan duration)
        {
            var running = true;

            var start = DateTime.UtcNow;

            var heartbeats = new List<Heartbeat>();

            while (running)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var now = DateTime.UtcNow;

                var elapsed = now - start;

                var iterations = _loadTest.TotalIterations;

                var throughput = iterations / elapsed.TotalSeconds;

                if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue;

                var heartbeat = new Heartbeat
                {
                    Timestamp = now,
                    Runtime = elapsed, // Run Duration
                    TotalRuntime = DateTime.Now - startTime,
                    Throughput = throughput,
                    TotalIterations = iterations,
                    TotalErrors = _loadTest.TotalErrors,
                    TotalThreads = _loadTest.TotalThreads
                };

                heartbeats.Add(heartbeat);

                OnHeartbeat(heartbeat);

                if (elapsed >= duration)
                {
                    running = false;
                }
                else if (DateTime.UtcNow - start < duration)
                {
                    Thread.Sleep(1000);
                }
            }

            return heartbeats;
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            var handler = Heartbeat;

            if (handler != null)
            {
                handler(this, heartbeat); //todo: add try catch?
            }
        }
    }
}
