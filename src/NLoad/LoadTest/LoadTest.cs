using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using NLoad.LoadTest;

// ui -> worker -> load test -> runners -> workers

namespace NLoad
{
    public sealed class LoadTest<T> : ILoadTest where T : ITest, new()
    {
        private long _threadCount;
        private long _totalErrors;
        private long _totalIterations;
        List<TestRunner<T>> _testRunners;
        private readonly HeartRateMonitor _monitor;
        private readonly ManualResetEvent _quitEvent;
        private readonly ManualResetEvent _startEvent;
        public event EventHandler<Heartbeat> Heartbeat;
        private readonly LoadTestConfiguration _configuration;
        private readonly CancellationToken _cancellationToken;

        [ExcludeFromCodeCoverage]
        public LoadTest(LoadTestConfiguration configuration, CancellationToken cancellationToken)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (cancellationToken == null)
                throw new ArgumentNullException("cancellationToken");

            _configuration = configuration;
            _cancellationToken = cancellationToken;

            _startEvent = new ManualResetEvent(false);
            _quitEvent = new ManualResetEvent(false);

            _monitor = new HeartRateMonitor(this, cancellationToken);
        }


        #region Properties

        public LoadTestConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public long TotalIterations
        {
            get
            {
                return Interlocked.Read(ref _totalIterations);
            }
        }

        public long TotalErrors
        {
            get
            {
                return Interlocked.Read(ref _totalErrors);
            }
        }

        public long ThreadCount
        {
            get
            {
                return Interlocked.Read(ref _threadCount);
            }
        }

        #endregion


        public LoadTestResult Run()
        {
            try
            {
                CreateTestRunners(_configuration.NumberOfThreads);

                StartTestRunners();

                Warmup();
                
                var stopWatch = Stopwatch.StartNew();

                StartLoadTest();

                var heartbeats = MonitorHeartRate();

                ShutdownTestRunners();

                stopWatch.Stop();

                return LoadTestResult(stopWatch.Elapsed, heartbeats);
            }
            catch (Exception e)
            {
                throw new NLoadException("An error occurred while running load test. See inner exception for details.", e);
            }
        }

        private void StartLoadTest()
        {
            _startEvent.Set();
        }


        public void IncrementIterationsCounter()
        {
            Interlocked.Increment(ref _totalIterations);
        }

        public void IncrementErrorsCounter()
        {
            Interlocked.Increment(ref _totalErrors);
        }

        public void IncrementThreadCount()
        {
            Interlocked.Increment(ref _threadCount);
        }


        private List<Heartbeat> MonitorHeartRate()
        {
            _monitor.Heartbeat += Heartbeat;

            var heartbeats = _monitor.Start();

            _monitor.Heartbeat -= Heartbeat;

            return heartbeats;
        }

        private void Warmup()
        {
            while (ThreadCount < _configuration.NumberOfThreads && !_cancellationToken.IsCancellationRequested)
            {
                if (Heartbeat != null)
                {
                    Heartbeat(this, new Heartbeat
                    {
                        TotalIterations = _totalIterations,
                        ThreadCount = ThreadCount
                    });
                }

                Thread.Sleep(1000);
            }
        }

        private LoadTestResult LoadTestResult(TimeSpan elapsed, List<Heartbeat> heartbeats)
        {
            var testRuns = _testRunners.Where(k => k.Result != null && k.Result.TestRuns != null)
                                       .SelectMany(k => k.Result.TestRuns)
                                       .ToList();

            var result = new LoadTestResult
            {
                TestRunnersResults = _testRunners.Where(k => k.Result != null).Select(k => k.Result),
                TotalIterations = _totalIterations,
                TotalRuntime = elapsed,
                TotalErrors = _totalErrors,
                Heartbeat = heartbeats,
                TestRuns = testRuns
            };

            if (testRuns.Any())
            {
                result.MaxResponseTime = testRuns.Max(k => k.ResponseTime);
                result.MinResponseTime = testRuns.Min(k => k.ResponseTime);
                result.AverageResponseTime = new TimeSpan(Convert.ToInt64((testRuns.Average(k => k.ResponseTime.Ticks))));
            }

            if (heartbeats.Any())
            {
                result.MaxThroughput = heartbeats.Where(k => !double.IsNaN(k.Throughput)).Max(k => k.Throughput);
                result.MinThroughput = heartbeats.Where(k => !double.IsNaN(k.Throughput)).Min(k => k.Throughput);
                result.AverageThroughput = heartbeats.Where(k => !double.IsNaN(k.Throughput)).Average(k => k.Throughput);
            }

            return result;
        }

        private void StartTestRunners()
        {
            _testRunners.ForEach(testRunner =>
            {
                testRunner.Initialize();

                testRunner.Run();
            });
        }

        private void CreateTestRunners(int count)
        {
            _testRunners = new List<TestRunner<T>>(count);

            for (var i = 0; i < count; i++)
            {
                var testRunner = new TestRunner<T>(this, _startEvent, _quitEvent);

                _testRunners.Add(testRunner);
            }
        }

        private void ShutdownTestRunners()
        {
            _quitEvent.Set();

            //todo: replace with Task.WaitAll
            while (_testRunners.Any(w => w.IsBusy))
            {
                Thread.Sleep(1); //todo: ???
            }
        }
    }
}

