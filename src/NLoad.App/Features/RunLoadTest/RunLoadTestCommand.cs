using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    [Obsolete]
    internal class RunLoadTestCommand : ICommand
    {
        private bool _canExecute = true;
        private readonly BackgroundWorker _worker;
        private readonly LoadTestViewModel _loadTestViewModel;

        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommand(LoadTestViewModel loadTestViewModel)
            : this(loadTestViewModel, new BackgroundWorker())
        {
        }

        public RunLoadTestCommand(LoadTestViewModel loadTestViewModel, BackgroundWorker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException("worker");
            }

            if (loadTestViewModel == null)
            {
                throw new ArgumentNullException("loadTestViewModel");
            }

            _loadTestViewModel = loadTestViewModel;
            _worker = worker;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            ChangeCanExecuteTo(false);

            Initialize();

            _worker.RunWorkerAsync();
        }

        #region Helpers

        private void Initialize()
        {
            _loadTestViewModel.Reset();

            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += RunLoadTest;
            _worker.RunWorkerCompleted += LoadTestCompleted;
        }

        private void RunLoadTest(object sender, DoWorkEventArgs e)
        {
            var loadTest = NLoad.Test<HttpRequestTest>()
                .WithNumberOfThreads(_loadTestViewModel.NumberOfThreads)
                .WithDurationOf(_loadTestViewModel.Duration)
                .WithDeleyBetweenThreadStart(_loadTestViewModel.DeleyBetweenThreadStart)
                .OnHeartbeat(OnHeartbeat)
                .Build();

            _loadTestViewModel.LoadTest = loadTest; //todo: replace with cancellation-token

            e.Result = loadTest.Run();
        }

        private void LoadTestCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (LoadTestResult) e.Result;

            MapLoadTestResultToViewModel(result);

            _worker.DoWork -= RunLoadTest;

            _worker.RunWorkerCompleted -= LoadTestCompleted;

            ChangeCanExecuteTo(true);
        }

        private void OnHeartbeat(object sender, Heartbeat e)
        {
            if (e == null) return;

            _loadTestViewModel.Heartbeats.Add(e);

            _loadTestViewModel.Throughput = Math.Round(e.Throughput, 0, MidpointRounding.AwayFromZero);
            _loadTestViewModel.Elapsed = FormatElapsed(e.Elapsed);
            _loadTestViewModel.TotalIterations = e.TotalIterations;
            _loadTestViewModel.TotalErrors = e.TotalErrors;

            _loadTestViewModel.ChartModel.InvalidatePlot(true);
        }

        private void ChangeCanExecuteTo(bool canExecute)
        {
            _canExecute = canExecute;

            if (CanExecuteChanged != null)
            {
                Application.Current.Dispatcher.Invoke(() =>

                    CanExecuteChanged(this, EventArgs.Empty));
            }
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            return string.Format("{0}:{1}:{2}", elapsed.ToString("hh"), elapsed.ToString("mm"), elapsed.ToString("ss"));
        }

        private void MapLoadTestResultToViewModel(LoadTestResult result)
        {
            _loadTestViewModel.Elapsed = FormatElapsed(result.TotalRuntime);
            _loadTestViewModel.TotalIterations = result.TotalIterations;
            _loadTestViewModel.MinThroughput = result.MinThroughput;
            _loadTestViewModel.MaxThroughput = result.MaxThroughput;
            _loadTestViewModel.AverageThroughput = result.AverageThroughput;
            _loadTestViewModel.TotalErrors = result.TotalErrors;
        }

        #endregion
    }
}