using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace NLoad
{
    public sealed class LoadTest<T> : ILoadTest where T : ITest, new()
    {
        private long _totalIterations;
        private bool _cancelled = false;
        List<TestRunner<T>> _testRunners;
        private readonly LoadTestConfiguration _configuration;
        private readonly List<Heartbeat> _heartbeat = new List<Heartbeat>();
        private readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public event EventHandler<Heartbeat> Heartbeat;

        [ExcludeFromCodeCoverage]
        public LoadTest(LoadTestConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
        }

        public LoadTestConfiguration Configuration
        {
            get { return _configuration; }
        }


        public LoadTestResult Run()
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();

                Initialize();

                StartTestRunners();

                MonitorHeartRate();

                Shutdown();

                WaitForTestRunners();

                stopWatch.Stop();

                #region Move to function

                var testRuns = _testRunners.Where(k => k.Result != null && k.Result.TestRuns != null)
                    .SelectMany(k => k.Result.TestRuns)
                    .ToList();

                var result = new LoadTestResult
                {
                    TestRunnersResults = _testRunners.Where(k => k.Result != null).Select(k => k.Result),
                    TotalIterations = _totalIterations,
                    TotalRuntime = stopWatch.Elapsed,
                    Heartbeat = _heartbeat,
                    TestRuns = testRuns
                };

                if (testRuns.Any())
                {
                    result.MaxResponseTime = testRuns.Max(k => k.ResponseTime);
                    result.MinResponseTime = testRuns.Min(k => k.ResponseTime);
                    result.AverageResponseTime =
                        new TimeSpan(Convert.ToInt64((testRuns.Average(k => k.ResponseTime.Ticks))));
                }

                if (_heartbeat.Any())
                {
                    result.MaxThroughput = _heartbeat.Where(k => !double.IsNaN(k.Throughput)).Max(k => k.Throughput);
                    result.MinThroughput = _heartbeat.Where(k => !double.IsNaN(k.Throughput)).Min(k => k.Throughput);
                    result.AverageThroughput =
                        _heartbeat.Where(k => !double.IsNaN(k.Throughput)).Average(k => k.Throughput);
                }

                #endregion

                return result;
            }
            catch (Exception e)
            {
                throw new NLoadException("An error occurred while running load test. See inner exception for details.", e);
            }
        }

        public void Cancel()
        {
            _cancelled = true;

            foreach (var testRunner in _testRunners)
            {
                testRunner.Cancel();
            }
        }


        private void Initialize()
        {
            CreateTestRunners(_configuration.NumberOfThreads);

            _testRunners.ForEach(k => k.Initialize());
        }

        private void StartTestRunners()
        {
            //todo: add error handling

            _testRunners.ForEach(k => k.Run());
        }

        private void CreateTestRunners(int count)
        {
            _testRunners = new List<TestRunner<T>>(count);

            for (var i = 0; i < count; i++)
            {
                var testRunner = new TestRunner<T>(this, _quitEvent);

                _testRunners.Add(testRunner);
            }
        }

        private void MonitorHeartRate()
        {
            var running = true;

            var start = DateTime.UtcNow;

            while (running)
            {
                var elapsed = DateTime.UtcNow - start;

                if (elapsed >= _configuration.Duration || _cancelled)
                {
                    running = false;
                }
                else
                {
                    var iterations = Interlocked.Read(ref _totalIterations);

                    var throughput = iterations / elapsed.TotalSeconds;

                    if (double.IsNaN(throughput) || double.IsInfinity(throughput)) continue;

                    OnHeartbeat(throughput, elapsed, iterations);

                    if (DateTime.UtcNow - start < _configuration.Duration) Thread.Sleep(1000);
                }
            }
        }

        private void Shutdown()
        {
            _quitEvent.Set();
        }

        private void WaitForTestRunners()
        {
            while (true)
            {
                if (_testRunners.Any(w => w.IsBusy))
                {
                    Thread.Sleep(1); //todo: ???
                }
                else
                {
                    break;
                }
            }
        }

        private void OnHeartbeat(double throughput, TimeSpan delta, long iterations)
        {
            var heartbeat = new Heartbeat(DateTime.UtcNow, throughput, delta, iterations);

            _heartbeat.Add(heartbeat);

            var handler = Heartbeat;

            if (handler != null)
            {
                handler(this, heartbeat); //todo: add try catch?
            }
        }

        public void IncrementIterationsCounter()
        {
            Interlocked.Increment(ref _totalIterations);
        }
    }
}

// ui -> worker -> load test -> runners -> worker