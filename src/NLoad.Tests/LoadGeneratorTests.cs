using Moq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class LoadGeneratorTests
    {
        [TestMethod]
        public void ShouldSetResultWhenCompleted()
        {
            var loadTest = new Mock<ILoadTest>();

            var context = new LoadTestContext();

            var loadGenerator = new LoadGenerator(loadTest.Object, typeof(TestMock), context, new CancellationToken());

            loadGenerator.Start();

            context.StartEvent.Set();
            
            Thread.Sleep(1);

            context.QuitEvent.Set();

            while (loadGenerator.IsBusy) { /* Wait */}

            Assert.IsNotNull(loadGenerator.Result);
        }
    }
}
