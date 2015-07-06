using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad.LoadTest
{
    public class HeartRateMonitor
    {
        private readonly ILoadTest _loadTest;
        private readonly CancellationToken _cancellationToken;

        private readonly List<Heartbeat> _heartbeat = new List<Heartbeat>();

        public event EventHandler<Heartbeat> Heartbeat;

        public HeartRateMonitor(ILoadTest loadTest, CancellationToken cancellationToken)
        {
            _loadTest = loadTest;
            _cancellationToken = cancellationToken;
        }

        public List<Heartbeat> Heartbeats
        {
            get
            {
                return _heartbeat;
            }
        }

        public void Start()//todo: return heartbeat list here instead of holding it
        {
            var running = true;

            var start = DateTime.UtcNow;

            while (running)
            {
                var elapsed = DateTime.UtcNow - start;

                var iterations = _loadTest.TotalIterations;
                var errors = _loadTest.TotalErrors;

                var throughput = iterations / elapsed.TotalSeconds;

                if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue;

                OnHeartbeat(throughput, elapsed, iterations, errors);

                if (elapsed >= _loadTest.Configuration.Duration || _cancellationToken.IsCancellationRequested)
                {
                    running = false;
                }
                else if (DateTime.UtcNow - start < _loadTest.Configuration.Duration)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnHeartbeat(double throughput, TimeSpan delta, long totalIterations, long totalErrors)
        {
            var heartbeat = new Heartbeat(DateTime.UtcNow, throughput, delta, totalIterations, totalErrors);

            _heartbeat.Add(heartbeat);

            var handler = Heartbeat;

            if (handler != null)
            {
                handler(this, heartbeat); //todo: add try catch?
            }
        }
    }
}
