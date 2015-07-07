using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLoad.LoadTest;
using System;
using System.Threading;

namespace NLoad.Tests
{
    [TestClass]
    public class HeartRateMonitorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var loadTest = new Mock<ILoadTest>();

            loadTest.Setup(k => k.TotalIterations).Returns(1);

            loadTest.Setup(k => k.TotalErrors).Returns(1);

            loadTest.Setup(k => k.Configuration).Returns(
                new LoadTestConfiguration
                {
                    Duration = TimeSpan.FromSeconds(3)
                });

            var cancellationToken = new CancellationTokenSource();

            var heartRateMonitor = new HeartRateMonitor(loadTest.Object, cancellationToken.Token);

            var heartbeats = heartRateMonitor.Start();

            Assert.IsNotNull(heartbeats);
            Assert.IsTrue(heartbeats.Any());
        }
    }
}
