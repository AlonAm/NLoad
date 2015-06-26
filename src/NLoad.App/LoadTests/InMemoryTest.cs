using System.Threading;

namespace NLoad.App
{
    internal sealed class InMemoryTest : ITest
    {
        public void Initialize()
        {
            //Console.WriteLine("Initialize test...");
        }

        public TestResult Execute()
        {
            Thread.Sleep(1);

            return TestResult.Default; // todo: turn to mutable
        }
    }
}