using System.Threading;

namespace NLoad.Tests
{
    public class TestRun : ITestRun
    {
        private static int _counter;

        public static int Counter
        {
            get
            {
                return _counter;
            }
        }

        public void Initialize()
        {
            Interlocked.Increment(ref _counter);
        }

        public void Execute()
        {
            Thread.Sleep(1);
        }
    }
}