using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NLoad
{
    public class LoadTest<T> where T : ITest, new()
    {
        private long _counter;
        private readonly LoadTestConfiguration _configuration;
        private readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private readonly List<TestRunResult> _testResults = new List<TestRunResult>();
        private readonly List<Heartbeat> _heartbeats = new List<Heartbeat>();

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
            get
            {
                return _configuration;
            }
        }

        public LoadTestResult Run()
        {
            var stopWatch = Stopwatch.StartNew();

            var result = new LoadTestResult();

            try
            {
                var threads = CreateThreads();

                StartThreads(threads);

                Monitor();

                ShutdownThreads(threads);

                result.Iterations = _counter;
                result.Runtime = stopWatch.Elapsed;
                result.TestRuns = _testResults;
                result.Heartbeats = _heartbeats;
            }
            catch (Exception e)
            {
                throw new LoadTestException("An error occurred while load testing. See inner exception for details.", e);
            }
            finally
            {
                stopWatch.Stop();
            }

            return result;
        }

        private void Monitor()
        {
            var running = true;

            var start = DateTime.Now;

            Interlocked.Exchange(ref _counter, 0);

            Thread.Sleep(1000);

            while (running)
            {
                var delta = DateTime.Now - start;

                var counter = Interlocked.Read(ref _counter);

                OnHeartbeat(throughput: counter / delta.TotalSeconds);

                if (delta >= _configuration.Duration)
                {
                    running = false;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void ThreadProc()
        {
            //todo: move to another class
            //public class ThreadProc
            //{
            //}

            var test = new T();

            test.Initialize();

            var results = new List<TestRunResult>();

            while (!_quitEvent.WaitOne(0))
            {
                var result = new TestRunResult
                {
                    StartTime = DateTime.Now,

                    TestResult = test.Execute(),

                    EndTime = DateTime.Now
                };

                Interlocked.Increment(ref _counter);

                results.Add(result);
            }

            lock (_testResults)
            {
                _testResults.AddRange(results);
            }
        }

        private List<Thread> CreateThreads()
        {
            var threads = new List<Thread>(_configuration.NumberOfThreads);

            for (var i = 0; i < _configuration.NumberOfThreads; i++)
            {
                var thread = new Thread(ThreadProc);

                threads.Add(thread);
            }

            return threads;
        }

        private void StartThreads(IEnumerable<Thread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Start();

                Thread.Sleep(_configuration.DelayBetweenThreadStart);
            }
        }

        private void ShutdownThreads(IEnumerable<Thread> threads)
        {
            _quitEvent.Set();

            foreach (var t in threads)
            {
                t.Join();
            }
        }

        protected virtual void OnHeartbeat(double throughput)
        {
            var heartbeat = new Heartbeat
            {
                Throughput = throughput
            };

            _heartbeats.Add(heartbeat);

            var handler = Heartbeat;

            if (handler != null) handler(this, heartbeat);
        }
    }
}