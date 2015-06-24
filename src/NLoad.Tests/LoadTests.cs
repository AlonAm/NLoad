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
            Assert.IsTrue(result.TestsResults.Any());
            Assert.IsTrue(result.Heartbeats.Any());
        }

        [TestMethod]
        public void NumberOfThreads()
        {
            const int numberOfThreads = 10;

            NLoad.Test<ThreadCounter>()
                    .WithNumberOfThreads(numberOfThreads)
                    .WithDurationOf(TimeSpan.Zero)
                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                        .Build()
                            .Run();

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