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

        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommandAsync(LoadTestViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            SetCanExecute(false);

            _viewModel.Reset();

            var progress = new Progress<Heartbeat>(_viewModel.HandleHeartbeat);

            var cancellationTokenSource = new CancellationTokenSource();

            _viewModel.CancellationTokenSource = cancellationTokenSource;

            var result = await RunLoadTestAsync(_viewModel.Configuration, cancellationTokenSource.Token, progress);

            _viewModel.HandleLoadTestResult(result);

            SetCanExecute(true);
        }

        #region Helpers

        private static Task<LoadTestResult> RunLoadTestAsync(
            LoadTestConfiguration configuration,
            CancellationToken cancellationToken,
            IProgress<Heartbeat> progress)
        {
            return Task.Run(() =>
            {
                var loadTest = NLoad.Test<InMemoryTest>()
                                        .WithNumberOfThreads(configuration.NumberOfThreads)
                                        .WithDurationOf(configuration.Duration)
                                        .WithDeleyBetweenThreadStart(configuration.DelayBetweenThreadStart)
                                        .WithCancellationToken(cancellationToken)
                                        .OnHeartbeat((s, e) => progress.Report(e))
                                    .Build();

                return loadTest.Run();

            }, cancellationToken);
        }

        private void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;

            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
