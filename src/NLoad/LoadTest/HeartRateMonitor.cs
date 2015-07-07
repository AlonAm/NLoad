using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad.LoadTest
{
    public class HeartRateMonitor
    {
        private readonly ILoadTest _loadTest;
        private readonly CancellationToken _cancellationToken;

        public event EventHandler<Heartbeat> Heartbeat;

        public HeartRateMonitor(ILoadTest loadTest, CancellationToken cancellationToken)
        {
            _loadTest = loadTest;
            _cancellationToken = cancellationToken;
        }

        public List<Heartbeat> Start()
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

                if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue; //todo: verify

                var heartbeat = new Heartbeat(now, throughput, elapsed, iterations, _loadTest.TotalErrors);

                heartbeats.Add(heartbeat);

                OnHeartbeat(heartbeat);

                if (elapsed >= _loadTest.Configuration.Duration || _cancellationToken.IsCancellationRequested)
                {
                    running = false;
                }
                else if (DateTime.UtcNow - start < _loadTest.Configuration.Duration)
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
