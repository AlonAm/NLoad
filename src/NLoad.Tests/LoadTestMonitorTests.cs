using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadTestMonitorTests
    {
        [TestMethod]
        public void MonitorHeartRate()
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

            var heartRateMonitor = new LoadTestMonitor(loadTest.Object, cancellationToken.Token);

            var heartbeats = heartRateMonitor.Start(DateTime.Now, TimeSpan.FromSeconds(2));

            Assert.IsNotNull(heartbeats);
            Assert.IsTrue(heartbeats.Any());
        }
    }
}
