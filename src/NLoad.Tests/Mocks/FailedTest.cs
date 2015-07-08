using System.Threading;

namespace NLoad.Tests
{
    public class FailedTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1);

            return TestResult.Failure;
        }
    }
}