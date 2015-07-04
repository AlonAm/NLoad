using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using OxyPlot;

namespace NLoad.App.Features.RunLoadTest
{
    internal class RunLoadTestCommand : ICommand
    {
        private bool _canExecute = true;
        private readonly BackgroundWorker _worker;
        private readonly LoadTestViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public RunLoadTestCommand(LoadTestViewModel viewModel)
            : this(viewModel, new BackgroundWorker())
        {
        }

        public RunLoadTestCommand(LoadTestViewModel viewModel, BackgroundWorker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException("worker");
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            _viewModel = viewModel;
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

            RunLoadTestAsync();
        }

        #region Helpers

        private void RunLoadTestAsync()
        {
            _worker.RunWorkerAsync();
        }

        private void Initialize()
        {
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += RunLoadTest;
            _worker.RunWorkerCompleted += LoadTestCompleted;
        }

        /// <summary>
        /// Run load test on background worker (thread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunLoadTest(object sender, DoWorkEventArgs e)
        {
            _viewModel.Points.Clear(); //todo: refactor

            var loadTest = NLoad.Test<InMemoryTest>()
                                    .WithNumberOfThreads(_viewModel.NumberOfThreads)
                                    .WithDurationOf(_viewModel.Duration)
                                    .WithDeleyBetweenThreadStart(_viewModel.DeleyBetweenThreadStart)
                                    .OnHeartbeat(OnHeartbeat)
                                .Build();

            _viewModel.LoadTest = loadTest;

            e.Result = loadTest.Run();
        }

        private void LoadTestCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (LoadTestResult)e.Result;

            MapLoadTestResultToViewModel(result);

            _worker.DoWork -= RunLoadTest;
            _worker.RunWorkerCompleted -= LoadTestCompleted;

            ChangeCanExecuteTo(true);
        }

        private void MapLoadTestResultToViewModel(LoadTestResult result)
        {
            _viewModel.Elapsed = FormatElapsed(result.TotalRuntime);

            _viewModel.Iterations = result.TotalIterations;

            _viewModel.MinThroughput = result.MinThroughput;

            _viewModel.MaxThroughput = result.MaxThroughput;

            _viewModel.AverageThroughput = result.AverageThroughput;
        }

        private void OnHeartbeat(object sender, Heartbeat e)
        {
            if (e == null) return;

            _viewModel.Throughput = Math.Round(e.Throughput, 2, MidpointRounding.AwayFromZero);
            _viewModel.Elapsed = FormatElapsed(e.Elapsed);
            _viewModel.Iterations = e.Iterations;

            _viewModel.Points.Add(new DataPoint(_viewModel.Iterations, _viewModel.Throughput));

            _viewModel.PlotModel.InvalidatePlot(true);
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

        #endregion
    }
}