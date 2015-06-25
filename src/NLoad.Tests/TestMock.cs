using System;
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

            return new TestResult(true);
        }
    }

    public class RandomDelayTest : ITest
    {
        readonly Random _random = new Random();

        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(_random.Next(1, 500));

            return new TestResult(true);
        }
    }

    public class OneSecondDelayTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1000);

            return new TestResult(true);
        }
    }
}