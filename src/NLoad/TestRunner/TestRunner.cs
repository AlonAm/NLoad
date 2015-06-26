using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        #region Fields

        private readonly ILoadTest _loadTest;
        private BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _quitEvent;

        #endregion

        #region Ctor

        public TestRunner(ILoadTest loadTest, ManualResetEvent quitEvent)
        {
            _loadTest = loadTest;
            _quitEvent = quitEvent;
        }

        #endregion

        #region Properties

        public TestRunnerResult Result { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _backgroundWorker.IsBusy;
            }
        }

        #endregion

        public void Initialize()
        {
            var backgroundWorker = new BackgroundWorker();

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

        private void RunOnBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            var context = (TestRunContext)e.Argument;

            var result = new TestRunnerResult(starTime: DateTime.UtcNow);

            var testRunResults = new List<TestRunResult>();

            long iterations = 0;

            var test = new T();

            test.Initialize();

            while (!context.QuitEvent.WaitOne(0))
            {
                var testRunResult = new TestRunResult
                {
                    StartTime = DateTime.UtcNow,
                    TestResult = test.Execute(),
                    EndTime = DateTime.UtcNow
                };

                _loadTest.IncrementCounter();

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