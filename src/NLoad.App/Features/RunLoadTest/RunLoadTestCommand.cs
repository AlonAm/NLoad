using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    internal class RunLoadTestCommand : ICommand
    {
        #region Fields

        private bool _canExecute = true;
        private BackgroundWorker _worker;
        private readonly LoadTestViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        #endregion

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
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            ChangeCanExecuteTo(false);

            Initialize();

            Start();
        }

        #region Helpers

        private void Start()
        {
            _worker.RunWorkerAsync();
        }

        private void Initialize()
        {
            _worker = new BackgroundWorker();

            _worker.DoWork += RunLoadTest;
            _worker.RunWorkerCompleted += OnLoadTestCompleted;
        }

        /// <summary>
        /// Run load test on background worker (thread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunLoadTest(object sender, DoWorkEventArgs e)
        {
            var loadTest = NLoad.Test<InMemoryTest>()
                                    .WithNumberOfThreads(_viewModel.NumberOfThreads)
                                    .WithDurationOf(_viewModel.Duration)
                                    .WithDeleyBetweenThreadStart(_viewModel.DeleyBetweenThreadStart)
                                    .OnHeartbeat(OnHeartbeat)
                                .Build();

            e.Result = loadTest.Run();
        }

        private void OnLoadTestCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (LoadTestResult)e.Result;

            SaveResult(result);

            _worker.DoWork -= RunLoadTest;
            _worker.RunWorkerCompleted -= OnLoadTestCompleted;
            
            _worker = null;

            ChangeCanExecuteTo(true);
        }

        private void SaveResult(LoadTestResult result)
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