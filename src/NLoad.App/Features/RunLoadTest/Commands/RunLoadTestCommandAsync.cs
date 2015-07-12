using NLoad.App.Tests;
using System;
using System.Diagnostics;
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

            _viewModel.CancellationTokenSource = new CancellationTokenSource();;

            var progress = new Progress<Heartbeat>(_viewModel.HandleHeartbeat);

            try
            {
                var result = await RunLoadTestAsync(_viewModel.Configuration, _viewModel.CancellationTokenSource.Token, progress);

                _viewModel.LoadTestResult = result;
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine("Cancelled");
            }
            finally
            {
                SetCanExecute(true);
            }
        }

        #region Helpers

        private  Task<LoadTestResult> RunLoadTestAsync(
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
