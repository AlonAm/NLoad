using System.Threading;

namespace NLoad.Tests
{
    public class TestMock : ITest
    {
        public void Initialize()
        {
        }

        public bool Execute()
        {
            Thread.Sleep(1);

            return true;
        }
    }
}