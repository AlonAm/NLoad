using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async void StartAsync()
        {
            IsBusy = true;

            var testRunnerResult = Task.Run(() => Start(_context), _cancellationToken)
                                       .ConfigureAwait(false);

            try
            {
                Result = await testRunnerResult;
            }
            catch (TaskCanceledException)
            {
                Result = new TestRunnerResult();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private TestRunnerResult Start(TestRunContext context)
        {
            var result = new TestRunnerResult(starTime: DateTime.UtcNow);

            var testRunResults = new List<TestRunResult>();

            var test = new T();

            test.Initialize();

            _loadTest.IncrementTotalThreads();

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

                    _loadTest.IncrementTotalIterations();

                    if (testRunResult.TestResult.Failed) //todo: ?
                    {
                        _loadTest.IncrementTotalErrors();
                    }
                }

                testRunResults.Add(testRunResult);
            }

            result.TestRuns = testRunResults;

            result.EndTime = DateTime.UtcNow;

            return result;
        }
    }
}