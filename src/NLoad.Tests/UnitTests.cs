using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    using System;

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void BuildLoadTest()
        {
            const int numberOfThreads = 10;
            var duration = TimeSpan.FromSeconds(1);
            var delayBetweenThreadStart = TimeSpan.FromMilliseconds(100);

            var loadTestBuilder = new LoadTestBuilder<MockTestRun>();

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
        public void RunLoadTest()
        {
            var loadTestBuilder = new LoadTestBuilder<MockTestRun>();

            var loadTest = loadTestBuilder
                            .WithNumberOfThreads(1)
                            .WithDurationOf(TimeSpan.FromMilliseconds(100))
                            .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                            .Build();

            var result = loadTest.Run();

            Assert.IsTrue(result.TotalTestRuns > 0);
        }

        [TestMethod]
        public void RunMultithreadedLoadTest()
        {
            var loadTestBuilder = new LoadTestBuilder<MockTestRun>();

            var loadTest = loadTestBuilder
                            .WithNumberOfThreads(10)
                            .WithDurationOf(TimeSpan.FromMilliseconds(100))
                            .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                            .Build();

            var result = loadTest.Run();

            //todo: assert number of threads
        }
    }

    public class MockTestRun : ITestRun
    {
        public void Initialize()
        {
        }

        public void Execute()
        {
            Thread.Sleep(100);
        }
    }
}