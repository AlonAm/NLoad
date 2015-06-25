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
            Thread.Sleep(1200);

            return new TestResult(true);
        }
    }
}