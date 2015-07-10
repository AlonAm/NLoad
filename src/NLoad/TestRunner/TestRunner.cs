using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NLoad
{
    public class TestRunner<T> where T : ITest, new()
    {
        private readonly ILoadTest _loadTest;
        private readonly TestRunContext _context;
        private readonly CancellationToken _cancellationToken;

        public TestRunner(ILoadTest loadTest, TestRunContext context, CancellationToken cancellationToken)
        {
            _loadTest = loadTest;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public TestRunnerResult Result { get; private set; }

        public bool IsBusy { get; private set; }

        public async void Start()
        {
            IsBusy = true;

            Result = await RunTestsAsync(_context).ConfigureAwait(false);

            IsBusy = false;
        }

        private Task<TestRunnerResult> RunTestsAsync(TestRunContext context)
        {
            return Task.Run(() =>
            {
                var result = new TestRunnerResult(starTime: DateTime.UtcNow);

                var testRunResults = new List<TestRunResult>();

                var test = new T();

                test.Initialize();

                _loadTest.IncrementThreadCount();

                context.StartEvent.WaitOne();

                while (!context.QuitEvent.WaitOne(0) && !_cancellationToken.IsCancellationRequested)
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

            });
        }
    }
}