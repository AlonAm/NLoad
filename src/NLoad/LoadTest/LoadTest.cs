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
        
        private DateTime _startTime;

        private readonly Type _testType;

        private readonly HeartRateMonitor _heartRateMonitor;

        private readonly LoadTestContext _context;

        private readonly LoadTestConfiguration _configuration;

        private List<LoadGenerator> _loadGenerators;

        private CancellationToken _cancellationToken;

        readonly Stopwatch _totalRuntimeStopWatch = new Stopwatch();

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

            _context = new LoadTestContext();

            _heartRateMonitor = new HeartRateMonitor(this);
        }

        public LoadTest(Type testType, LoadTestConfiguration configuration, CancellationToken cancellationToken)
            : this(testType, configuration)
        {
            _cancellationToken = cancellationToken;

            _heartRateMonitor.CancellationToken = _cancellationToken;

            _cancellationToken.Register(() =>
            {
                if (Aborted != null)
                {
                    Aborted(this, new EventArgs());
                }
            });
        }

        #endregion

        /// <summary>
        /// Run Load Test
        /// </summary>
        public LoadTestResult Run()
        {
            try
            {
                FireStartingEvent();

                _startTime = DateTime.Now;

                TryCreateLoadGenerators();

                TryStartLoadGenerators();

                TryWarmup();

                MeasureTotalRuntime();

                TryStart();

                TryMonitorHeartRate();

                TryShutdown();

                FireFinishedEvent();

                var result = new LoadTestResultBuilder(this, _loadGenerators, _heartRateMonitor)
                    .Build();

                return result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NLoadException("An error occurred while running load test.", ex);
            }
        }

        private void MeasureTotalRuntime()
        {
            _totalRuntimeStopWatch.Restart();

            Finished += StopMeasuringTotalRuntime;
        }

        private void StopMeasuringTotalRuntime(object sender, EventArgs e)
        {
            _totalRuntimeStopWatch.Stop();

            TotalRuntime = _totalRuntimeStopWatch.Elapsed;

            Finished -= StopMeasuringTotalRuntime;
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

        public long TotalThreads
        {
            get
            {
                return Interlocked.Read(ref _threadCount);
            }
        }

        public TimeSpan TotalRuntime
        {
            get;
            private set;
        }

        #endregion

        #region Error Handling

        private void TryStart()
        {
            Start();
        }

        private void TryCreateLoadGenerators()
        {
            try
            {
                CreateLoadGenerators();
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to create load generators.", ex);
            }
        }

        private void TryStartLoadGenerators()
        {
            try
            {
                StartLoadGenerators();
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to start load generators.", ex);
            }
        }

        private void TryWarmup()
        {
            try
            {
                Warmup();
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to warmup load test.", ex);
            }
        }

        private void TryMonitorHeartRate()
        {
            try
            {
                MonitorHeartRate();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to monitor load test heart rate.", ex);
            }
        }

        private void TryShutdown()
        {
            try
            {
                Shutdown();
            }
            catch (Exception ex)
            {
                throw new NLoadException("Failed to shutdown load test.", ex);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Start Load Test
        /// </summary>
        private void Start()
        {
            _context.StartEvent.Set();
        }

        /// <summary>
        /// Create Load Generators.
        /// </summary>
        private void CreateLoadGenerators()
        {
            _loadGenerators = new List<LoadGenerator>(_configuration.NumberOfThreads);

            for (var i = 0; i < _configuration.NumberOfThreads; i++)
            {
                var testRunner = new LoadGenerator(this, _testType, _context, _cancellationToken);

                _loadGenerators.Add(testRunner);
            }
        }

        /// <summary>
        /// Start Load Generators.
        /// </summary>
        private void StartLoadGenerators()
        {
            _loadGenerators.ForEach(testRunner => testRunner.Start());
        }

        /// <summary>
        /// Warmup Load Test
        /// </summary>
        private void Warmup()
        {
            while (TotalThreads < _configuration.NumberOfThreads && !_cancellationToken.IsCancellationRequested)
            {
                if (Heartbeat != null)
                {
                    Heartbeat(this, new Heartbeat
                    {
                        TotalIterations = _totalIterations,
                        TotalRuntime = DateTime.Now - _startTime,
                        TotalThreads = TotalThreads
                    });
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Monitor load test heartbeat every one second
        /// </summary>
        /// <returns></returns>
        private void MonitorHeartRate()
        {
            _heartRateMonitor.Heartbeat += Heartbeat;

            _heartRateMonitor.Start(_startTime, _configuration.Duration);

            _heartRateMonitor.Heartbeat -= Heartbeat;
        }

        /// <summary>
        /// Shutdown Load Test
        /// </summary>
        private void Shutdown()
        {
            _context.QuitEvent.Set();

            while (_loadGenerators.Any(w => w.IsBusy)) //todo: replace with Task.WaitAll
            {
                Thread.Sleep(1);
            }
        }

        private void FireStartingEvent()
        {
            if (Starting != null)
            {
                Starting(this, new EventArgs());
            }
        }

        private void FireFinishedEvent()
        {
            if (Finished != null)
            {
                Finished(this, new EventArgs());
            }
        }

        internal void IncrementTotalIterations()
        {
            Interlocked.Increment(ref _totalIterations);
        }

        internal void IncrementTotalErrors()
        {
            Interlocked.Increment(ref _totalErrors);
        }

        internal void IncrementTotalThreads()
        {
            Interlocked.Increment(ref _threadCount);
        }

        #endregion
    }
}