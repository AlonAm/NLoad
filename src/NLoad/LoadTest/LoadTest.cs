using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NLoad
{
    public sealed class LoadTest<T> : ILoadTest where T : ITest, new()
    {
        #region Fields

        private long _threadCount;
        private long _totalErrors;
        private long _totalIterations;
        private List<TestRunner<T>> _testRunners;
        private readonly LoadTestMonitor _monitor;
        private readonly ManualResetEvent _quitEvent;
        private readonly ManualResetEvent _startEvent;
        public event EventHandler<Heartbeat> Heartbeat;
        private readonly LoadTestConfiguration _configuration;
        private readonly CancellationToken _cancellationToken;

        #endregion

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

            _monitor = new LoadTestMonitor(this, cancellationToken);
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
                Initialize();

                StartTestRunners();

                Warmup();

                var stopWatch = Stopwatch.StartNew();

                StartLoadTest();

                var heartbeats = MonitorLoadTest();

                Shutdown();

                stopWatch.Stop();

                return LoadTestResult(stopWatch.Elapsed, heartbeats);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException) throw;
                
                throw new NLoadException("An error occurred while running load test. See inner exception for details.", e);
            }
        }

        public void IncrementTotalIterations()
        {
            Interlocked.Increment(ref _totalIterations);
        }

        public void IncrementTotalErrors()
        {
            Interlocked.Increment(ref _totalErrors);
        }

        public void IncrementTotalThreads()
        {
            Interlocked.Increment(ref _threadCount);
        }

        // helpers

        private void StartLoadTest()
        {
            _startEvent.Set();
        }

        private List<Heartbeat> MonitorLoadTest()
        {
            _monitor.Heartbeat += Heartbeat;

            var heartbeats = _monitor.Start(_configuration.Duration);

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
            _testRunners.ForEach(testRunner => testRunner.StartAsync());
        }

        private void Initialize()
        {
            var context = new TestRunContext
            {
                StartEvent = _startEvent,
                QuitEvent = _quitEvent
            };

            _testRunners = new List<TestRunner<T>>(_configuration.NumberOfThreads);

            for (var i = 0; i < _configuration.NumberOfThreads; i++)
            {
                var testRunner = new TestRunner<T>(this, context, _cancellationToken);

                _testRunners.Add(testRunner);
            }
        }

        private void Shutdown()
        {
            _quitEvent.Set();

            while (_testRunners.Any(w => w.IsBusy)) //todo: replace with Task.WaitAll
            {
                Thread.Sleep(1);
            }
        }
    }
}

