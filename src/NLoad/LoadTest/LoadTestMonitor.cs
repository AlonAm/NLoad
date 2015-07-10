using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad
{
    public class LoadTestMonitor
    {
        private readonly ILoadTest _loadTest;
        private readonly CancellationToken _cancellationToken;

        public event EventHandler<Heartbeat> Heartbeat;

        public LoadTestMonitor(ILoadTest loadTest, CancellationToken cancellationToken)
        {
            _loadTest = loadTest;
            _cancellationToken = cancellationToken;
        }

        public List<Heartbeat> Start(TimeSpan duration)
        {
            var running = true;

            var start = DateTime.UtcNow;

            var heartbeats = new List<Heartbeat>();

            while (running)
            {
                var now = DateTime.UtcNow;

                var elapsed = now - start;

                var iterations = _loadTest.TotalIterations;

                var throughput = iterations / elapsed.TotalSeconds;

                if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue;

                var heartbeat = new Heartbeat
                {
                    Timestamp = now,
                    Elapsed = elapsed,
                    Throughput = throughput,
                    TotalIterations = iterations,
                    TotalErrors = _loadTest.TotalErrors,
                    ThreadCount = _loadTest.ThreadCount
                };

                heartbeats.Add(heartbeat);

                OnHeartbeat(heartbeat);

                if (elapsed >= duration || _cancellationToken.IsCancellationRequested)
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
