using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private readonly ILoadTest _loadTest;
        private BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _quitEvent;

        public TestRunner(ILoadTest loadTest, ManualResetEvent quitEvent)
        {
            _loadTest = loadTest;
            _quitEvent = quitEvent;
        }

        public TestRunnerResult Result { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _backgroundWorker.IsBusy;
            }
        }

        public void Initialize()
        {
            var backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += RunOnBackgroundWorker;
            backgroundWorker.RunWorkerCompleted += BindResponseFromWorkerThread;

            _backgroundWorker = backgroundWorker;
        }

        public void Run()
        {
            var context = new TestRunContext
            {
                QuitEvent = _quitEvent
            };

            _backgroundWorker.RunWorkerAsync(context);
        }

        private void RunOnBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            long iterations = 0; //todo: is this needed?
            var worker = (BackgroundWorker)sender;
            var context = (TestRunContext)e.Argument;
            var result = new TestRunnerResult(starTime: DateTime.UtcNow);
            var testRunResults = new List<TestRunResult>();

            var test = new T();

            test.Initialize();

            while (!context.QuitEvent.WaitOne(0))
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    result.EndTime = DateTime.UtcNow;
                    e.Result = result;
                    return;
                }

                var testRunResult = new TestRunResult
                {
                    StartTime = DateTime.UtcNow
                };

                try
                {
                    testRunResult.TestResult = test.Execute();
                }
                catch
                {
                    testRunResult.TestResult = TestResult.Failed;
                }
                finally
                {
                    testRunResult.EndTime = DateTime.UtcNow;
                }

                if (!testRunResult.TestResult.Passed) //todo: refactor?
                {
                    _loadTest.IncrementErrorsCounter();
                }

                _loadTest.IncrementIterationsCounter();

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
            if (e.Cancelled) return;

            var result = e.Result as TestRunnerResult;

            if (result != null)
            {
                Result = result;
            }
        }

        public void Cancel()
        {
            _backgroundWorker.CancelAsync();
        }
    }
}