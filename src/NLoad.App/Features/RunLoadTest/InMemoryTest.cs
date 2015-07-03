using System.Threading;

namespace NLoad.App.Features.RunLoadTest
{
    internal sealed class InMemoryTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(10);

            return TestResult.Success;
        }
    }
}