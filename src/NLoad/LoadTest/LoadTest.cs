using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NLoad
{
    public class LoadTest<T> : LoadTest where T : ITest, new()
    {
        public LoadTest(LoadTestConfiguration configuration, CancellationToken cancellationToken)
            : base(typeof(T), configuration, cancellationToken)
        {
        }
    }

    public class LoadTest : ILoadTest
    {
        #region Fields

        private long _threadCount;
        private long _totalErrors;
        private long _totalIterations;

        private readonly Type _testType;
        private readonly LoadTestMonitor _monitor;
        //private readonly ManualResetEvent _quitEvent;
        //private readonly ManualResetEvent _startEvent;
        private readonly LoadTestContext _loadTestContext;
        private readonly LoadTestConfiguration _configuration;

        private List<TestRunner> _testRunners;
        private CancellationToken _cancellationToken;


        #endregion

        #region Public Events

        public event EventHandler<Heartbeat> Heartbeat;

        public event EventHandler<EventArgs> Starting;

        public event EventHandler<EventArgs> Finished;

        public event EventHandler<EventArgs> Aborted;

        #endregion

        #region Ctor

        public LoadTest(Type testType, LoadTestConfiguration configuration)
        {
            if (testType == null) 
                throw new ArgumentNullException("testType");

            if (configuration == null) 
                throw new ArgumentNullException("configuration");

            _testType = testType;
            _configuration = configuration;

            _loadTestContext = new LoadTestContext
            {
                StartEvent = new ManualResetEvent(false),
                QuitEvent = new ManualResetEvent(false)
            };

            _monitor = new LoadTestMonitor(this);
        }

        public LoadTest(Type testType, LoadTestConfiguration configuration, CancellationToken cancellationToken)
            : this(testType, configuration)
        {
            _cancellationToken = cancellationToken;

            _monitor.CancellationToken = _cancellationToken;

            _cancellationToken.Register(() =>
            {
                if (Aborted != null)
                {
                    Aborted(this, new EventArgs());
                }
            });
        }

        #endregion

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

        public long TotalThreads
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
                if (Starting != null)
                {
                    Starting(this, new EventArgs());
                }

                var startTime = DateTime.Now;

                CreateTestRunners();

                StartTestRunners();

                Warmup(startTime);

                var stopWatch = Stopwatch.StartNew();

                StartLoadTest();

                var heartbeats = MonitorLoadTest(startTime);

                Shutdown();

                stopWatch.Stop();

                if (Finished != null)
                {
                    Finished(this, new EventArgs());
                }

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
            _loadTestContext.StartEvent.Set();
        }

        private List<Heartbeat> MonitorLoadTest(DateTime startTime)
        {
            _monitor.Heartbeat += Heartbeat;

            var heartbeats = _monitor.Start(startTime, _configuration.Duration);

            _monitor.Heartbeat -= Heartbeat;

            return heartbeats;
        }

        private void Warmup(DateTime startTime)
        {
            while (TotalThreads < _configuration.NumberOfThreads && !_cancellationToken.IsCancellationRequested)
            {
                if (Heartbeat != null)
                {
                    Heartbeat(this, new Heartbeat
                    {
                        TotalIterations = _totalIterations,
                        TotalRuntime = DateTime.Now - startTime,
                        TotalThreads = TotalThreads
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
            _testRunners.ForEach(testRunner => testRunner.Start());
        }

        private void CreateTestRunners()
        {
            _testRunners = new List<TestRunner>(_configuration.NumberOfThreads);

            for (var i = 0; i < _configuration.NumberOfThreads; i++)
            {
                var testRunner = new TestRunner(this, _testType, _loadTestContext, _cancellationToken);

                _testRunners.Add(testRunner);
            }
        }

        private void Shutdown()
        {
            _loadTestContext.QuitEvent.Set();

            while (_testRunners.Any(w => w.IsBusy)) //todo: replace with Task.WaitAll
            {
                Thread.Sleep(1);
            }
        }
    }
}