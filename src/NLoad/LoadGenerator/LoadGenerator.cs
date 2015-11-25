using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NLoad
{
    public class LoadGenerator
    {
        private readonly Type _testType;
        private readonly LoadTest _loadTest;
        private readonly LoadTestContext _context;
        private readonly CancellationToken _cancellationToken;
        private int _isBusy;
        public LoadGenerator(LoadTest loadTest, Type testType, LoadTestContext context, CancellationToken cancellationToken)
        {
            _loadTest = loadTest;
            _testType = testType;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public bool IsBusy
        {
            get { return _isBusy != 0; }
        }

        public LoadGeneratorResult Result { get; private set; }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref _isBusy, 1, 0) != 0) return;

            Task.Run(() => Start(_context), _cancellationToken)
                            .ContinueWith(task =>
                            {
                                Result = task.IsFaulted || task.IsCanceled ? new LoadGeneratorResult() : task.Result;
                                Interlocked.Exchange(ref _isBusy, 0);
                            },
                            _cancellationToken);
        }

        private LoadGeneratorResult Start(LoadTestContext context)
        {
            var result = new LoadGeneratorResult(starTime: DateTime.UtcNow);

            var testRunResults = new List<TestRunResult>();

            var test = (ITest)Activator.CreateInstance(_testType);

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