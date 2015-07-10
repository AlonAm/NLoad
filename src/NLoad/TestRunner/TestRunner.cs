using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private readonly ILoadTest _loadTest;
        private readonly ManualResetEvent _startEvent;
        private readonly ManualResetEvent _quitEvent;

        public TestRunner(ILoadTest loadTest, ManualResetEvent startEvent, ManualResetEvent quitEvent)
        {
            IsBusy = false;
            _loadTest = loadTest;
            _startEvent = startEvent;
            _quitEvent = quitEvent;
        }

        public TestRunnerResult Result { get; private set; }

        public bool IsBusy { get; private set; }

        public async void Run()
        {
            IsBusy = true;

            var context = new TestRunContext
            {
                StartEvent = _startEvent,
                QuitEvent = _quitEvent
            };

            Result = await Task.Run(() => RunTests(context));

            IsBusy = false;
        }

        private TestRunnerResult RunTests(TestRunContext context)
        {
            var result = new TestRunnerResult(starTime: DateTime.UtcNow);

            var testRunResults = new List<TestRunResult>();

            var test = new T();

            test.Initialize();

            _loadTest.IncrementThreadCount();

            context.StartEvent.WaitOne();

            while (!context.QuitEvent.WaitOne(0))
            {
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
                    testRunResult.TestResult = TestResult.Failure;
                }
                finally
                {
                    testRunResult.EndTime = DateTime.UtcNow;
                }

                if (testRunResult.TestResult.Failed) //todo: refactor?
                {
                    _loadTest.IncrementErrorsCounter();
                }

                _loadTest.IncrementIterationsCounter();

                testRunResults.Add(testRunResult);
            }

            result.TestRuns = testRunResults;

            result.EndTime = DateTime.UtcNow;

            return result;
        }
    }
}