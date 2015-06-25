using System;
using System.Collections.Generic;
using System.Threading;

namespace NLoad
{
    internal class ThreadProc<T> where T : ITest, new()
    {
        private static long _counter;
        private readonly ManualResetEvent _quitEvent;
        private readonly List<TestRunResult> _testRunResults;

        public ThreadProc(long counter, ManualResetEvent quitEvent, List<TestRunResult> testRunResults)
        {
            _counter = counter;
            _quitEvent = quitEvent;
            _testRunResults = testRunResults;
        }

        public void Start()
        {
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

            lock (_testRunResults)
            {
                _testRunResults.AddRange(results);
            }
        }
    }
}