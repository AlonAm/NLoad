using System;
using System.ComponentModel;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class StopLoadTestCommand : ICommand
    {
        private readonly BackgroundWorker _worker;

        public StopLoadTestCommand(BackgroundWorker worker)
        {
            _worker = worker;
        }

        public bool CanExecute(object parameter)
        {
            return _worker != null && _worker.IsBusy;
        }

        public void Execute(object parameter)
        {
            if (_worker != null && _worker.IsBusy)
            {
                _worker.CancelAsync();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}