using System.Threading;

namespace NLoad.Tests
{
    public class ThreadCounter : ITest
    {
        private static int _threadCount;

        public static int Count
        {
            get
            {
                return _threadCount;
            }
        }

        public void Initialize()
        {
            Interlocked.Increment(ref _threadCount);
        }

        public TestResult Execute()
        {
            Thread.Sleep(1);

            return TestResult.Success;
        }
    }
}