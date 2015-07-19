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

        public void Execute(object parameter)
        {
            if (_isRunning)
            {
                Cancel();
            }
            else
            {
                Initialize();

                IsRunning(true);

                RunLoadTest()
                    .ContinueWith(loadTest =>
                    {
                        if (!loadTest.IsFaulted && !loadTest.IsCanceled)
                        {
                            _viewModel.Result = loadTest.Result;
                        }

                        IsRunning(false);
                    });
            }
        }

        private Task<LoadTestResult> RunLoadTest()
        {
            var progress = _progress as IProgress<Heartbeat>;

            return Task.Run(() =>
            {
                var loadTest = NLoad.Test(_viewModel.SelectedLoadTest)
                                        .WithNumberOfThreads(_viewModel.Configuration.NumberOfThreads)
                                        .WithRunDurationOf(_viewModel.Configuration.Duration)
                                        .WithDeleyBetweenThreadStart(_viewModel.Configuration.DelayBetweenThreadStart)
                                        .WithCancellationToken(_cancellationTokenSource.Token)
                                        .OnHeartbeat((s, e) => progress.Report(e))
                                    .Build();

                return loadTest.Run();

            }, _cancellationTokenSource.Token);
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _progress = new Progress<Heartbeat>(_viewModel.HandleHeartbeat);

            _viewModel.Result = null;

            if (_viewModel.Heartbeats == null)
            {
                _viewModel.Heartbeats = new List<Heartbeat>();
            }
            else
            {
                _viewModel.Heartbeats.Clear();
            }
        }

        private void Cancel()
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
