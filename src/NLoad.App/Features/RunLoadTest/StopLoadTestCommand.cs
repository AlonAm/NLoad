using System;
using System.Threading;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class StopLoadTestCommand : ICommand
    {
        private readonly CancellationTokenSource _tokenSource;

        public event EventHandler CanExecuteChanged;

        public StopLoadTestCommand(CancellationTokenSource tokenSource)
        {
            _tokenSource = tokenSource;
        }

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
            _tokenSource.Cancel();
        }
    }
}