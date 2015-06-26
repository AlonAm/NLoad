using System.Threading;

namespace NLoad.Tests
{
    public class OneSecondDelayTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1100);

            return new TestResult(true);
        }
    }
}