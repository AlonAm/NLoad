using System;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class StopLoadTestCommand : ICommand
    {
        private readonly LoadTestViewModel _viewModel;
        public event EventHandler CanExecuteChanged;

        public StopLoadTestCommand(LoadTestViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.CancellationTokenSource.Cancel();
        }
    }
}