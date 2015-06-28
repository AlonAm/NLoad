﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    internal class RunLoadTestCommand : ICommand
    {
        private bool _canExecute = true;
        private BackgroundWorker _worker;
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
            var loadTest = NLoad.Test<InMemoryTest>()
                                    .WithNumberOfThreads(_viewModel.NumberOfThreads)
                                    .WithDurationOf(_viewModel.Duration)
                                    .WithDeleyBetweenThreadStart(_viewModel.DeleyBetweenThreadStart)
                                    .OnHeartbeat(OnHeartbeat)
                                .Build();

            e.Result = loadTest.Run();
        }

        private void LoadTestCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (LoadTestResult)e.Result;

            MapResultToViewModel(result);

            _worker.DoWork -= RunLoadTest;

            _worker.RunWorkerCompleted -= LoadTestCompleted;

            ChangeCanExecuteTo(true);
        }

        private void MapResultToViewModel(LoadTestResult result)
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