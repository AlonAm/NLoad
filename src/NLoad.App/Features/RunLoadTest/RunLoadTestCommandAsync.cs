using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class RunLoadTestCommandAsync : ICommand
    {
        private bool _canExecute = true;
        private readonly LoadTestViewModel _viewModel;
        private readonly CancellationToken _cancellationToken;

        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommandAsync(LoadTestViewModel viewModel, CancellationToken cancellationToken)
        {
            _viewModel = viewModel;
            _cancellationToken = cancellationToken;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            CanExecute(false);

            _viewModel.Reset();

            var progressHandler = new Progress<Heartbeat>(_viewModel.OnHeartbeat);

            var progress = progressHandler as IProgress<Heartbeat>;

            var loadTestResult = await Task.Run(() =>
            {
                var loadTest = NLoad.Test<InMemoryTest>()
                                    .WithNumberOfThreads(_viewModel.NumberOfThreads)
                                    .WithDurationOf(_viewModel.Duration)
                                    .WithDeleyBetweenThreadStart(_viewModel.DeleyBetweenThreadStart)
                                    .OnHeartbeat((s, e) => progress.Report(e))
                                    .Build();

                var result = loadTest.Run();

                return result;
            
            }, _cancellationToken);

            _viewModel.OnLoadTestCompleted(loadTestResult);

            CanExecute(true);
        }

        private void CanExecute(bool canExecute)
        {
            _canExecute = canExecute;

            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
