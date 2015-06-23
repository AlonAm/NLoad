using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTestBuilderTests
    {
        [TestMethod]
        public void BuildLoadTest()
        {
            const int numberOfThreads = 10;
            var duration = TimeSpan.FromSeconds(1);
            var delayBetweenThreadStart = TimeSpan.FromMilliseconds(100);

            var loadTestBuilder = new LoadTestBuilder<TestMock>();

            var loadTest = loadTestBuilder
                            .WithNumberOfThreads(numberOfThreads)
                            .WithDurationOf(duration)
                            .WithDeleyBetweenThreadStart(delayBetweenThreadStart)
                            .Build();

            Assert.IsNotNull(loadTest.Configuration);
            Assert.AreEqual(numberOfThreads, loadTest.Configuration.NumberOfThreads);
            Assert.AreEqual(duration, loadTest.Configuration.Duration);
            Assert.AreEqual(delayBetweenThreadStart, loadTest.Configuration.DelayBetweenThreadStart);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            const int numberOfThreads = 10;
            var duration = TimeSpan.FromSeconds(1);
            var delayBetweenThreadStart = TimeSpan.FromMilliseconds(100);

            var loadTest = NLoad.Test<TestMock>()
                                    .WithNumberOfThreads(numberOfThreads)
                                    .WithDurationOf(duration)
                                    .WithDeleyBetweenThreadStart(delayBetweenThreadStart)
                                .Build();
            
            var result = loadTest.Run();

            Assert.IsNotNull(result);
        }
    }
}