using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTests
    {
        [TestMethod]
        public void RunLoadTest()
        {
            const int numberOfThreads = 10;

            var loadTest = NLoad.Test<TestMock>()
                                    .WithNumberOfThreads(numberOfThreads)
                                    .WithDurationOf(TimeSpan.Zero)
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                .Build();

            var result = loadTest.Run();

            Assert.AreEqual(numberOfThreads, TestMock.ThreadCounter);

            Assert.IsTrue(result.TotalTestRuns > 0);

            Assert.IsTrue(result.TotalRuntime > TimeSpan.Zero);
        }

        [TestMethod]
        public void ShouldFireCurrentThroughputEvent()
        {
            var eventFired = false;
            double throughput = 0;

            var loadTest = NLoad.Test<TestMock>()
                                    .WithDurationOf(TimeSpan.FromSeconds(1))
                                    .OnCurrentThroughput((s, t) =>
                                    {
                                        eventFired = true;
                                        throughput = t;
                                    })
                                .Build();

            loadTest.Run();

            Assert.IsTrue(eventFired);
            Assert.IsTrue(throughput > 0);
        }
    }
}