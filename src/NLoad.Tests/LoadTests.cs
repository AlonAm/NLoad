using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTests
    {
        [TestMethod]
        public void SingleThreadLoadTest()
        {
            var duration = TimeSpan.FromSeconds(3);

            var loadTest = NLoad.Test<OneSecondDelayTest>()
                                    .WithNumberOfThreads(1)
                                    .WithDurationOf(duration)
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build();

            var result = loadTest.Run();

            Assert.AreNotEqual(0, result.TotalIterations);
            Assert.AreNotEqual(TimeSpan.Zero, result.TotalRuntime);
            Assert.IsTrue(result.TotalRuntime > duration);
            
            Assert.IsTrue(result.Heartbeat.Any());

            //Assert.IsTrue(result.MinThroughput > 0);
            Assert.IsTrue(result.MaxThroughput > 0);
            Assert.IsTrue(result.AverageThroughput > 0);

            Assert.IsTrue(result.MinResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.MaxResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.AverageResponseTime > TimeSpan.Zero);

            //Assert.IsTrue(result.TestRuns.Any());
        }

        [TestMethod]
        public void MultithreadedLoadTest()
        {
            var duration = TimeSpan.FromSeconds(3);

            var loadTest = NLoad.Test<OneSecondDelayTest>()
                                    .WithNumberOfThreads(10)
                                    .WithDurationOf(duration)
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build();

            var result = loadTest.Run();

            Assert.AreNotEqual(0, result.TotalIterations);

            Assert.AreNotEqual(TimeSpan.Zero, result.TotalRuntime);

            Assert.IsTrue(result.TotalRuntime > duration);

            Assert.IsTrue(result.Heartbeat.Any());

//            Assert.IsTrue(result.MinThroughput > 0);
            Assert.IsTrue(result.MaxThroughput > 0);
            Assert.IsTrue(result.AverageThroughput > 0);

            Assert.IsTrue(result.MinResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.MaxResponseTime > TimeSpan.Zero);
            Assert.IsTrue(result.AverageResponseTime > TimeSpan.Zero);

            //Assert.IsTrue(result.TestRuns.Any());
        }

        [TestMethod]
        public void HeartbeatCountEqualsDurationInSeconds()
        {
            for (var durationInSeconds = 1; durationInSeconds < 3; durationInSeconds++)
            {
                var duration = TimeSpan.FromSeconds(durationInSeconds);

                var result = NLoad.Test<TestMock>()
                                    .WithNumberOfThreads(1)
                                    .WithDurationOf(duration)
                                    .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                                        .Build()
                                            .Run();

                Assert.AreEqual(durationInSeconds, result.Heartbeat.Count);
            }
        }

        [TestMethod]
        public void OneIteration()
        {
            TestNumberOfIterations(durationInSeconds: 1, numThreads: 1);
        }

        [TestMethod]
        public void TwoIterations()
        {
            TestNumberOfIterations(durationInSeconds: 2, numThreads: 1);
        }

        [TestMethod]
        public void ThreeIterations()
        {
            TestNumberOfIterations(durationInSeconds: 3, numThreads: 1);
        }

        private static void TestNumberOfIterations(int durationInSeconds, int numThreads)
        {
            var duration = TimeSpan.FromSeconds(durationInSeconds);

            var result = NLoad.Test<OneSecondDelayTest>()
                .WithNumberOfThreads(numThreads)
                .WithDurationOf(duration)
                .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                .Build()
                .Run();

            Assert.AreEqual(numThreads * durationInSeconds, result.TotalIterations);
        }

        [TestMethod]
        public void ActualNumberOfThreads()
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

            NLoad.Test<TestMock>()
                    .WithNumberOfThreads(1)
                    .WithDurationOf(TimeSpan.FromSeconds(1))
                    .OnHeartbeat((s, hearbeat) =>
                        {
                            eventFired = true;
                            throughput = hearbeat.Throughput;
                            Debug.WriteLine(hearbeat.Throughput);
                        })
                    .Build()
                    .Run();

            Assert.IsTrue(eventFired);
            Assert.AreNotEqual(0, throughput);
        }
    }
}