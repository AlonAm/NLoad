using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private static long _totalIterations;

        private readonly ManualResetEvent _quitEvent;
        private BackgroundWorker _backgroundWorker;

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
                return Interlocked.Read(ref _totalIterations);
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

        /// <summary>
        /// Runs on backgroundworker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Run(object sender, DoWorkEventArgs e)
        {
            var context = e.Argument as TestRunContext;

            if (context == null)
            {
                throw new LoadTestException("TestRunContext is null.");
            }

            var result = new TestRunnerResult(starTime: DateTime.Now);

            var testRunResults = new List<TestRunResult>();

            long iterations = 0;

            var test = new T();

            test.Initialize();

            while (!context.QuitEvent.WaitOne(0))
            {
                var testRunResult = new TestRunResult();

                testRunResult.StartTime = DateTime.Now;

                testRunResult.TestResult = test.Execute(); //todo: add try-catch?

                testRunResult.EndTime = DateTime.Now;

                Interlocked.Increment(ref _totalIterations);

                iterations++;

                testRunResults.Add(testRunResult);
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
