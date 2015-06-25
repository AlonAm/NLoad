using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace NLoad
{
    public class LoadTest<T> where T : ITest, new()
    {
        private readonly LoadTestConfiguration _configuration;
        private readonly List<Heartbeat> _heartbeat = new List<Heartbeat>();
        private readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public event EventHandler<Heartbeat> Heartbeat;

        #region Ctor

        [ExcludeFromCodeCoverage]
        public LoadTest(LoadTestConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
        }

        #endregion

        #region Properties

        public LoadTestConfiguration Configuration
        {
            get { return _configuration; }
        }

        #endregion

        public LoadTestResult Run()
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();

                var testRunners = CreateAndInitializeTestRunners(_configuration.NumberOfThreads, _quitEvent);

                StartTestRunners(testRunners);

                MonitorHeartRate();

                ShutdownTestRunners();

                WaitForTestRunners(testRunners);

                stopWatch.Stop();

                return new LoadTestResult
                {
                    TestRunnersResults = testRunners.Select(k => k.Result),
                    TotalIterations = testRunners.Where(k => k.Result != null).Sum(k => k.Result.Iterations), //TestRunner<T>.Counter, 
                    TotalRuntime = stopWatch.Elapsed,
                    Heartbeat = _heartbeat
                };
            }
            catch (Exception e)
            {
                throw new LoadTestException("An error occurred while running load test. See inner exception for details.", e);
            }
        }

        private static List<TestRunner<T>> CreateAndInitializeTestRunners(int count, ManualResetEvent quitEvent)
        {
            var testRunners = new List<TestRunner<T>>(count);

            for (var i = 0; i < count; i++)
            {
                var testRunner = new TestRunner<T>(quitEvent);

                testRunner.Initialize();

                testRunners.Add(testRunner);
            }

            return testRunners;
        }

        private static void StartTestRunners(List<TestRunner<T>> testRunners)
        {
            testRunners.ForEach(k => k.Start()); //todo: add error handling
        }

        private void MonitorHeartRate()
        {
            var running = true;

            var start = DateTime.Now;

            while (running)
            {
                var delta = DateTime.Now - start;

                if (delta >= _configuration.Duration)
                {
                    running = false;
                }
                else
                {
                    var throughput = TestRunner<T>.Counter / delta.TotalSeconds;

                    OnHeartbeat(throughput);

                    if (DateTime.Now - start < _configuration.Duration)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void ShutdownTestRunners()
        {
            _quitEvent.Set();
        }

        private static void WaitForTestRunners(List<TestRunner<T>> testRunners)
        {
            while (true)
            {
                if (testRunners.Any(w => w.IsBusy))
                {
                    //Thread.Sleep(1);
                }
                else
                {
                    break;
                }
            }
        }

        protected virtual void OnHeartbeat(double throughput)
        {
            var heartbeat = new Heartbeat(timestamp: DateTime.Now, throughput: throughput);

            _heartbeat.Add(heartbeat);

            var handler = Heartbeat;

            if (handler != null)
            {
                handler(this, heartbeat); //todo: add try catch?
            }
        }
    }
}


//private LoadTestResult CreateLoadTestResult(TimeSpan totalRuntime)
//{
//    var result = new LoadTestResult
//    {
//        TotalIterations = _counter,
//        TotalRuntime = totalRuntime,
//        TestRuns = _testRunResults,
//        Heartbeat = _heartbeats,
//        MinThroughput = _heartbeats.Min(k => k.Throughput),
//        MaxThroughput = _heartbeats.Max(k => k.Throughput),
//        AverageThroughput = _heartbeats.Average(k => k.Throughput),
//        MinResponseTime = _testRunResults.Min(k => k.ResponseTime),
//        MaxResponseTime = _testRunResults.Max(k => k.ResponseTime),
//        AverageResponseTime = new TimeSpan(Convert.ToInt64(_testRunResults.Average(k => k.ResponseTime.Ticks)))
//    };

//    return result;
//}

//private void ThreadProc()
//{
//    //todo: move to another class
//    //public class ThreadProc
//    //{
//    //}

//    var test = new T();

//    test.Initialize();

//    var results = new List<TestRunResult>();

//    while (!_quitEvent.WaitOne(0))
//    {
//        var result = new TestRunResult
//        {
//            StartTime = DateTime.Now,

//            TestResult = test.Execute(),

//            EndTime = DateTime.Now
//        };

//        Interlocked.Increment(ref _counter);

//        results.Add(result);
//    }

//    lock (_testRunResults)
//    {
//        _testRunResults.AddRange(results);
//    }
//}

//private List<BackgroundWorker> CreateThreads(int numberOfThreads)
//{
//    var backgroundWorkers = new List<BackgroundWorker>(numberOfThreads);

//    for (var i = 0; i < numberOfThreads; i++)
//    {
//        var backgroundWorker = new BackgroundWorker();

//        backgroundWorker.DoWork += OnDoWork;

//        backgroundWorker.RunWorkerCompleted += OnRunWorkerCompleted;

//        backgroundWorker.ProgressChanged += ProgressChanged;

//        backgroundWorkers.Add(backgroundWorker);
//    }

//    return backgroundWorkers;
//}

//private void ProgressChanged(object sender, ProgressChangedEventArgs e)
//{

//}

//private void OnDoWork(object sender, DoWorkEventArgs e)
//{
//    var context = (TestRunContext)e.Argument;

//    var proc = new ThreadProc<T>(context);

//    e.Result = proc.Start();


//    //e.Result = new ThreadResult
//    //{
//    //    Iterations = iterations
//    //};
//}

//private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//{
//    var result = (ThreadResult)e.Result;

//    _counter = _counter + result.Iterations;
//}

//private void StartThreads(IEnumerable<BackgroundWorker> threads, TimeSpan delay)
//{
//    foreach (var thread in threads)
//    {
//        var context = new TestRunContext
//        {
//            QuitEvent = _quitEvent
//        };

//        //var threadProc = new ThreadProc<T>(_counter, quitEvent, _testRunResults);

//        thread.RunWorkerAsync(context);

//        Thread.Sleep(delay);
//    }
//}

//private void ShutdownThreads(IEnumerable<BackgroundWorker> threads)
//{
//    _quitEvent.Set();

//    //foreach (var t in threads)
//    //{
//    //    t.Join();
//    //}
//}