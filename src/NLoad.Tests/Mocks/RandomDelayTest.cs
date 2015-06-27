using System;
using System.Threading;

namespace NLoad.Tests
{
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
}