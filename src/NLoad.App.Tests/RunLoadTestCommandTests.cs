using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLoad.App.Features.RunLoadTest;

namespace NLoad.App.Tests
{
    [TestClass]
    public class RunLoadTestCommandTests
    {
        [TestMethod]
        public void CanExecuteIsTrueByDefault()
        {
            var viewModel = new LoadTestViewModel();

            var command = new RunLoadTestCommand(viewModel);

            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowIfViewModelIsNull()
        {
            var runLoadTestCommand = new RunLoadTestCommand(null);

            Assert.IsNull(runLoadTestCommand);
        }

        [TestMethod]
        public void ShouldUpdateViewModel()
        {
            var viewModel = new LoadTestViewModel
            {
                SelectedLoadTest = typeof(InMemoryTest100),
                Configuration = new LoadTestConfiguration
                {
                    NumberOfThreads = 1,
                    Duration = TimeSpan.FromSeconds(1)
                }
            };

            var runLoadTestCommand = new RunLoadTestCommand(viewModel);

            runLoadTestCommand.Execute(null);

            WaitForLoadTestResult(viewModel);

            Assert.IsNotNull(viewModel.Heartbeat);
            Assert.IsNotNull(viewModel.Heartbeats);
            Assert.IsTrue(viewModel.Heartbeats.Any());
        }

        private static void WaitForLoadTestResult(LoadTestViewModel viewModel)
        {
            var start = DateTime.Now;

            while (viewModel.LoadTestResult == null)
            {
                if (DateTime.Now.Subtract(start) > TimeSpan.FromSeconds(5)) throw new TimeoutException();

                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }
        }
    }
}
