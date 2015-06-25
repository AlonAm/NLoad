using System;
using System.Diagnostics;
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
            var loadTest = NLoad.Test<OneSecondDelayTest>()
                                    .WithNumberOfThreads(5)
                                    .WithDurationOf(TimeSpan.FromSeconds(1))
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build();

            var result = loadTest.Run();

            Assert.AreNotEqual(0, result.TotalIterations);
            
            Assert.AreNotEqual(TimeSpan.Zero, result.TotalRuntime);
            
            //Assert.IsTrue(result.Heartbeat.Any());

            //Assert.IsTrue(result.TestRuns.Any());

            //Assert.IsTrue(result.MinThroughput > 0);
            //Assert.IsTrue(result.MaxThroughput > 0);
            //Assert.IsTrue(result.AverageThroughput > 0);

            //Assert.IsTrue(result.MinResponseTime > TimeSpan.Zero);
            //Assert.IsTrue(result.MaxResponseTime > TimeSpan.Zero);
            //Assert.IsTrue(result.AverageResponseTime > TimeSpan.Zero);
        }

        [TestMethod]
        public void CorrectNumberOfIterations()
        {
            var loadTest = NLoad.Test<OneSecondDelayTest>()
                                    .WithNumberOfThreads(1)
                                    .WithDurationOf(TimeSpan.FromSeconds(1))
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build();

            var result = loadTest.Run();

            Assert.AreEqual(1, result.TotalIterations);
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
                                    .WithNumberOfThreads(1)
                                    .WithDurationOf(TimeSpan.FromSeconds(5))
                                    .OnHeartbeat((s, hearbeat) =>
                                    {
                                        eventFired = true;
                                        throughput = hearbeat.Throughput;
                                        Debug.WriteLine(hearbeat.Throughput);
                                    })
                                .Build();

            loadTest.Run();

            Assert.IsTrue(eventFired);
            Assert.AreNotEqual(0, throughput);
        }
    }
}