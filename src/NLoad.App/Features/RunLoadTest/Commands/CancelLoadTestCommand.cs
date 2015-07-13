using System;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class CancelLoadTestCommand : ICommand
    {
        private readonly LoadTestViewModel _viewModel;
        public event EventHandler CanExecuteChanged;

        public CancelLoadTestCommand(LoadTestViewModel viewModel)
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