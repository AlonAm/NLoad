using System.Threading;

namespace NLoad.Tests
{
    public class TestRun : ITestRun
    {
        private static int _threadCounter;

        public static int ThreadCounter
        {
            get
            {
                return _threadCounter;
            }
        }

        public void Initialize()
        {
            Interlocked.Increment(ref _threadCounter);
        }

        public void Execute()
        {
            Thread.Sleep(1);
        }
    }
}