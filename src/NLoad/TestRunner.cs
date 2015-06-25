using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _quitEvent;

        // ReSharper disable once StaticFieldInGenericType
        // Every TestRunner<T> has its own instance of _totalIterations
        private static long _totalIterations;

        public TestRunner(ManualResetEvent quitEvent)
        {
            _quitEvent = quitEvent;
        }

        #region Properties

        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        public TestRunnerResult Result { get; private set; }

        public static long Counter
        {
            get { return Interlocked.Read(ref _totalIterations); }
        }

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
            var context = e.Argument as TestRunContext;

            if (context == null)
            {
                throw new ArgumentNullException("e", "Argument is null or is not of type TestRunContext");
            }

            var result = new TestRunnerResult(starTime: DateTime.Now);

            var testRunResults = new List<TestRunResult>();

            long iterations = 0;

            var test = new T();

            test.Initialize();

            while (!context.QuitEvent.WaitOne(0))
            {
                var testRunResult = new TestRunResult
                {
                    StartTime = DateTime.Now,

                    TestResult = test.Execute(), //todo: add try-catch?

                    EndTime = DateTime.Now
                };

                Interlocked.Increment(ref _totalIterations);

                iterations++;

                testRunResults.Add(testRunResult);
            }

            result.TestRuns = testRunResults;

            result.EndTime = DateTime.Now;

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
