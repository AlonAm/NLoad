using System;
using System.Collections.Generic;
using System.Linq;

namespace NLoad
{
    public class LoadTestResultBuilder
    {
        private readonly LoadTest _loadTest;
        private readonly List<LoadGenerator> _loadGenerators;
        private readonly HeartRateMonitor _heartRateMonitor;

        public LoadTestResultBuilder(LoadTest loadTest, List<LoadGenerator> loadGenerators, HeartRateMonitor heartRateMonitor)
        {
            if (loadTest == null)
            {
                throw new ArgumentNullException("loadTest");
            }

            if (loadGenerators == null)
            {
                throw new ArgumentNullException("loadGenerators");
            }

            if (heartRateMonitor == null)
            {
                throw new ArgumentNullException("heartRateMonitor");
            }

            _loadTest = loadTest;
            _loadGenerators = loadGenerators;
            _heartRateMonitor = heartRateMonitor;
        }

        public LoadTestResult Build()
        {
            try
            {
                var testRuns = _loadGenerators.Where(k => k.Result != null && k.Result.TestRuns != null)
                    .SelectMany(k => k.Result.TestRuns)
                    .ToList();

                var result = new LoadTestResult
                {
                    TestRunnersResults = _loadGenerators.Where(k => k.Result != null).Select(k => k.Result),
                    TotalIterations = _loadTest.TotalIterations,
                    TotalRuntime = _loadTest.TotalRuntime,
                    TotalErrors = _loadTest.TotalErrors,
                    Heartbeat = _heartRateMonitor.Heartbeats,
                    TestRuns = testRuns
                };

                if (testRuns.Any())
                {
                    result.MaxResponseTime = testRuns.Max(k => k.ResponseTime);
                    result.MinResponseTime = testRuns.Min(k => k.ResponseTime);
                    result.AverageResponseTime = new TimeSpan(Convert.ToInt64((testRuns.Average(k => k.ResponseTime.Ticks))));
                }

                if (_heartRateMonitor.Heartbeats != null && _heartRateMonitor.Heartbeats.Any())
                {
                    result.MaxThroughput = _heartRateMonitor.Heartbeats.Where(k => !double.IsNaN(k.Throughput)).Max(k => k.Throughput);
                    result.MinThroughput = _heartRateMonitor.Heartbeats.Where(k => !double.IsNaN(k.Throughput)).Min(k => k.Throughput);
                    result.AverageThroughput = _heartRateMonitor.Heartbeats.Where(k => !double.IsNaN(k.Throughput)).Average(k => k.Throughput);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to build load test result.", ex);
            }
        }
    }
}
