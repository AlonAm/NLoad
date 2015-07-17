using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                Configuration = new LoadTestConfiguration
                {
                    NumberOfThreads = 1,
                    Duration = TimeSpan.FromSeconds(1)
                }
            };

            var runLoadTestCommand = new RunLoadTestCommand(viewModel);

            runLoadTestCommand.Execute(null);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsNotNull(viewModel.Heartbeat);
            Assert.IsNotNull(viewModel.Heartbeats);
            Assert.IsTrue(viewModel.Heartbeats.Any());
        }
    }
}
