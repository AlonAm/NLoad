using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        #region Fields

        private BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _quitEvent;

        // ReSharper disable once StaticFieldInGenericType
        // Every TestRunner<T> has its own instance of _totalIterations
        private static long _totalIterations;

        #endregion

        #region Ctor

        public TestRunner(ManualResetEvent quitEvent)
        {
            _quitEvent = quitEvent;
        }

        #endregion

        #region Properties

        public static long TotalIterations
        {
            get
            {
                return Interlocked.Read(ref _totalIterations);
            }
        }


        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        public TestRunnerResult Result { get; private set; }
        
        #endregion

        public void Initialize()
        {
            var backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };

            backgroundWorker.DoWork += RunOnBackgroundWorker;

            backgroundWorker.RunWorkerCompleted += BindResponseFromWorkerThread;

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

        private static void RunOnBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            var context = (TestRunContext)e.Argument;

            var result = new TestRunnerResult(starTime: DateTime.UtcNow);

            var testRunResults = new List<TestRunResult>();

            long iterations = 0;

            var test = new T();

            test.Initialize();

            while (!context.QuitEvent.WaitOne(0))
            {
                var testRunResult = new TestRunResult();

                testRunResult.StartTime = DateTime.UtcNow;

                testRunResult.TestResult = test.Execute();

                testRunResult.EndTime = DateTime.UtcNow;

                Interlocked.Increment(ref _totalIterations);

                iterations++;

                testRunResults.Add(testRunResult);
            }

            result.TestRuns = testRunResults;

            result.EndTime = DateTime.UtcNow;

            result.Iterations = iterations;

            e.Result = result;
        }

        private void BindResponseFromWorkerThread(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as TestRunnerResult;

            if (result != null)
            {
                Result = result;
            }
        }
    }
}