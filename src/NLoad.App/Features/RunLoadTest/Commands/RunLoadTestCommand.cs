using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class RunLoadTestCommand : ICommand
    {
        private bool _isRunning;
        Progress<Heartbeat> _progress;
        private readonly LoadTestViewModel _viewModel;
        private CancellationTokenSource _cancellationTokenSource;
        
        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommand(LoadTestViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (_isRunning)
            {
                CancelLoadTest();

                return;
            }

            Initialize();

            IsRunning(true);

            try
            {
                var task = RunLoadTestAsync(_viewModel.SelectedLoadTest, _viewModel.Configuration, _cancellationTokenSource.Token, _progress);

                var result = await task;

                _viewModel.LoadTestResult = result;
            }
            catch (OperationCanceledException)
            {
                // Canceled
            }
            finally
            {
                IsRunning(false);
            }
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _progress = new Progress<Heartbeat>(_viewModel.HandleHeartbeat);

            if (_viewModel.Heartbeats != null)
            {
                _viewModel.Heartbeats.Clear();
            }
            else
            {
                _viewModel.Heartbeats = new List<Heartbeat>();
            }
        }

        private static Task<LoadTestResult> RunLoadTestAsync(
            Type testType,
            LoadTestConfiguration configuration,
            CancellationToken cancellationToken,
            IProgress<Heartbeat> progress)
        {
            return Task.Run(() =>
            {
                var loadTest = NLoad.Test(testType)
                                        .WithNumberOfThreads(configuration.NumberOfThreads)
                                        .WithRunDurationOf(configuration.Duration)
                                        .WithDeleyBetweenThreadStart(configuration.DelayBetweenThreadStart)
                                        .WithCancellationToken(cancellationToken)
                                        .OnHeartbeat((s, e) => progress.Report(e))
                                    .Build();

                return loadTest.Run();

            }, cancellationToken);
        }

        private void CancelLoadTest()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void IsRunning(bool isRunning)
        {
            _isRunning = isRunning;
            _viewModel.IsRunning = isRunning;
            _viewModel.RunButtonText = isRunning ? "Cancel" : "Run";
        }
    }
}
