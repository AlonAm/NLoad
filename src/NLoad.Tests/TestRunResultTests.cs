using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class TestRunResultTests
    {
        [TestMethod]
        public void ResponseTime()
        {
            var testRunResult = new TestRunResult
            {
                StartTime = DateTime.UtcNow
            };

            Thread.Sleep(1);

            testRunResult.EndTime = DateTime.UtcNow;

            Assert.AreEqual(testRunResult.EndTime - testRunResult.StartTime, testRunResult.ResponseTime);
        }
    }
}
