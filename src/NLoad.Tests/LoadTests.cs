using System;
using System.Linq;
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
            Assert.IsTrue(result.Iterations > 0);
            Assert.IsTrue(result.Runtime > TimeSpan.Zero);
            Assert.IsTrue(result.TestsResults.Any());
            Assert.IsTrue(result.Heartbeats.Any());
        }

        [TestMethod]
        public void ShouldFireCurrentThroughputEvent()
        {
            var eventFired = false;
            double throughput = 0;

            var loadTest = NLoad.Test<TestMock>()
                                    .WithDurationOf(TimeSpan.FromSeconds(1))
                                    .OnHeartbeat((s, args) =>
                                    {
                                        eventFired = true;
                                        throughput = args.Throughput;
                                    })
                                .Build();

            loadTest.Run();

            Assert.IsTrue(eventFired);
            Assert.IsTrue(throughput > 0);
        }
    }
}