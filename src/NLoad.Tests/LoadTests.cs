using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTests
    {
        [TestMethod]
        public void LoadTestResult()
        {
            var loadTest = NLoad.Test<TestMock>()
                                    .WithNumberOfThreads(1)
                                    .WithDurationOf(TimeSpan.Zero)
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build();

            var result = loadTest.Run();

            Assert.IsTrue(result.Iterations > 0);
            Assert.IsTrue(result.Runtime > TimeSpan.Zero);
            Assert.IsTrue(result.TestRuns.Any());
            Assert.IsTrue(result.Heartbeats.Any());

            Assert.IsTrue(result.MinThroughput > 0);
            Assert.IsTrue(result.MaxThroughput > 0);
            Assert.IsTrue(result.AverageThroughput > 0);

            Assert.IsTrue(result.MinResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.MaxResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.AverageResponseTime > TimeSpan.Zero);
        }

        [TestMethod]
        public void NumberOfThreads()
        {
            const int numberOfThreads = 10;

            var loadTest = NLoad.Test<ThreadCounter>()
                    .WithNumberOfThreads(numberOfThreads)
                    .WithDurationOf(TimeSpan.Zero)
                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                        .Build();

            loadTest.Run();

            Assert.AreEqual(numberOfThreads, ThreadCounter.Count);
        }

        [TestMethod]
        public void HeartbeatEvent()
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