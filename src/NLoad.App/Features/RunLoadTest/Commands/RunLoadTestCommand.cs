using NLoad.App.Tests;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class RunLoadTestCommand : ICommand
    {
        private bool _isRunning;
        private readonly LoadTestViewModel _viewModel;
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommand(LoadTestViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

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

            _viewModel.Reset();

            _cancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<Heartbeat>(_viewModel.HandleHeartbeat);

            _isRunning = true;

            try
            {
                var result = await RunLoadTestAsync(_viewModel.Configuration, _cancellationTokenSource.Token, progress);

                _viewModel.LoadTestResult = result;
            }
            catch (OperationCanceledException)
            {
                // Canceled
            }
            finally
            {
                _isRunning = false;
            }
        }

        private static Task<LoadTestResult> RunLoadTestAsync(LoadTestConfiguration configuration, CancellationToken cancellationToken, IProgress<Heartbeat> progress)
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

        private void CancelLoadTest()
        {
            if (_cancellationTokenSource == null)
            {
                throw new NullReferenceException("Invalid cancellation token source");
            }

            _cancellationTokenSource.Cancel();
        }
    }
}
