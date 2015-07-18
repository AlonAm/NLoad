using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTestBuilderTests
    {
        [TestMethod]
        public void VerifyLoadTestConfiguration()
        {
            const int numberOfThreads = 1;
            var duration = TimeSpan.FromSeconds(1);
            var delayBetweenThreadStart = TimeSpan.FromMilliseconds(100);

            var loadTest = NLoad.Test<TestMock>()
                            .WithNumberOfThreads(numberOfThreads)
                            .WithRunDurationOf(duration)
                            .WithDeleyBetweenThreadStart(delayBetweenThreadStart)
                            .Build();

            var configuration = loadTest.Configuration;

            Assert.IsNotNull(configuration);
            Assert.AreEqual(numberOfThreads, configuration.NumberOfThreads);
            Assert.AreEqual(duration, configuration.Duration);
            Assert.AreEqual(delayBetweenThreadStart, configuration.DelayBetweenThreadStart);
        }
    }
}