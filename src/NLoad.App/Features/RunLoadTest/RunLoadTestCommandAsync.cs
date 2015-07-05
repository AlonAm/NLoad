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

            var progressHandler = new Progress<Heartbeat>(heartbeat =>
            {
                MapHeartbeatToViewModel(heartbeat, _viewModel);

                _viewModel.ChartModel.InvalidatePlot(true);
            });

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

            MapResultToViewModel(loadTestResult, _viewModel);

            CanExecute(true);
        }

        private static void MapHeartbeatToViewModel(Heartbeat heartbeat, LoadTestViewModel viewModel)
        {
            viewModel.Heartbeats.Add(heartbeat);

            viewModel.Throughput = Math.Round(heartbeat.Throughput, 0, MidpointRounding.AwayFromZero);
            viewModel.Elapsed = FormatElapsed(heartbeat.Elapsed);
            viewModel.TotalIterations = heartbeat.TotalIterations;
            viewModel.TotalErrors = heartbeat.TotalErrors;
        }

        private static void MapResultToViewModel(LoadTestResult result, LoadTestViewModel viewModel)
        {
            //todo: move to extension

            viewModel.Elapsed = FormatElapsed(result.TotalRuntime);
            viewModel.TotalIterations = result.TotalIterations;
            viewModel.MinThroughput = result.MinThroughput;
            viewModel.MaxThroughput = result.MaxThroughput;
            viewModel.AverageThroughput = result.AverageThroughput;
            viewModel.TotalErrors = result.TotalErrors;
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            return string.Format("{0}:{1}:{2}", elapsed.ToString("hh"), elapsed.ToString("mm"), elapsed.ToString("ss"));
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
