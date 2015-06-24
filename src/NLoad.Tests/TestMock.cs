using System.Threading;

namespace NLoad.Tests
{
    public class TestMock : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1);

            return new TestResult
            {
                Passed = true
            };
        }
    }
}