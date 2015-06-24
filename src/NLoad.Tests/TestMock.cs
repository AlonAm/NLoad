using System;
using System.Threading;

namespace NLoad.Tests
{
    public class TestMock : ITest
    {
        readonly Random _random = new Random();

        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(_random.Next(1000));

            return new TestResult(true);
        }
    }
}