using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private readonly ManualResetEvent _quitEvent;
        private BackgroundWorker _backgroundWorker;

        private static long _counter;

        public TestRunner(ManualResetEvent quitEvent)
        {
            _quitEvent = quitEvent;
        }

        public bool IsBusy
        {
            get
            {
                return _backgroundWorker.IsBusy;
            }
        }

        public TestRunnerResult Result { get; private set; }

        public static long Counter
        {
            get
            {
                return Interlocked.Read(ref _counter);
            }
        }

        public void Initialize()
        {
            var backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };

            backgroundWorker.DoWork += Run;

            backgroundWorker.RunWorkerCompleted += OnWorkerCompleted;

            _backgroundWorker = backgroundWorker;
        }

        public void Start()
        {
            var context = new TestRunContext
            {
                QuitEvent = _quitEvent
            };

            _backgroundWorker.RunWorkerAsync(context);
        }

        private void Run(object sender, DoWorkEventArgs e)
        {
            var context = e.Argument as TestRunContext;

            if (context == null)
            {
                throw new LoadTestException("TestRunContext is null.");
            }

            var result = new TestRunnerResult(starTime: DateTime.Now);

            var testRunResults = new List<TestRunResult>();

            long iterations = 0;

            while (!context.QuitEvent.WaitOne(0))
            {
                var testRunResult = new TestRunResult(startTime: DateTime.Now);


                Thread.Sleep(1000); //execute test here

                testRunResult.TestResult = new TestResult(passed: true); //execute test here


                testRunResult.EndTime = DateTime.Now;

                testRunResults.Add(testRunResult);

                Interlocked.Increment(ref _counter);

                iterations++;

                _backgroundWorker.ReportProgress(0, iterations);
            }

            result.TestRuns = testRunResults;
            result.EndTime = DateTime.Now;
            result.Iterations = iterations;

            e.Result = result;
        }

        private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = (TestRunnerResult)e.Result;
        }
    }
}
