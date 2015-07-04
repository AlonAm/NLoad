using System;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class StopLoadTestCommand : ICommand
    {
        private readonly LoadTestViewModel _loadTestViewModel;

        public StopLoadTestCommand(LoadTestViewModel loadTestViewModel)
        {
            _loadTestViewModel = loadTestViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _loadTestViewModel != null;
        }

        public void Execute(object parameter)
        {
            _loadTestViewModel.CancelLoadTest();
        }

        public event EventHandler CanExecuteChanged;
    }
}