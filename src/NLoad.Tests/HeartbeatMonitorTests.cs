using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace NLoad.Tests
{
    [TestClass]
    public class HeartbeatMonitorTests
    {
        [TestMethod]
        public void MonitorHeartRate()
        {
            var config = new LoadTestConfiguration
            {
                Duration = TimeSpan.FromSeconds(3)
            };

            var loadTest = new Mock<ILoadTest>();

            loadTest.Setup(k => k.TotalIterations).Returns(1);
            loadTest.Setup(k => k.TotalErrors).Returns(1);
            loadTest.Setup(k => k.Configuration).Returns(config);

            var heartRateMonitor = new HeartRateMonitor(loadTest.Object);

            heartRateMonitor.Start(DateTime.Now, TimeSpan.FromSeconds(2));

            Assert.IsNotNull(heartRateMonitor.Heartbeats);

            Assert.IsTrue(heartRateMonitor.Heartbeats.Any());
        }
    }
}
