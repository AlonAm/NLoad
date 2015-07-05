using System.Threading;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class LoadTestViewModel : INotifyPropertyChanged
    {
        private string _elapsed;
        private long _totalErrors;
        private long _totalIterations;
        private double _minThroughput;
        private double _maxThroughput;
        private double _averageThroughput;
        private double _throughput;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            
            var token = cancellationTokenSource.Token;

            Heartbeats = new List<Heartbeat>();
            ChartModel = new LoadTestChart(Heartbeats);

            RunLoadTestCommand = new RunLoadTestCommandAsync(this, token);

            StopLoadTestCommand = new StopLoadTestCommand(cancellationTokenSource);

            Elapsed = "00:00:00";
            NumberOfThreads = 10;
            Duration = TimeSpan.FromSeconds(30);
            DeleyBetweenThreadStart = TimeSpan.Zero;
        }

        #region Properties

        public List<Heartbeat> Heartbeats { get; set; }

        public ILoadTest LoadTest { get; set; } //todo: replace with cancellationtoken

        // Commands

        public ICommand RunLoadTestCommand { get; private set; }

        public ICommand StopLoadTestCommand { get; private set; }

        // Display

        public double Throughput
        {
            get { return _throughput; }
            set
            {
                _throughput = value;
                OnPropertyChanged();
            }
        }

        public string Elapsed
        {
            get { return _elapsed; }
            set
            {
                _elapsed = value;
                OnPropertyChanged();
            }
        }

        public long TotalIterations
        {
            get { return _totalIterations; }
            set
            {
                _totalIterations = value;
                OnPropertyChanged();
            }
        }

        public long TotalErrors
        {
            get { return _totalErrors; }
            set
            {
                _totalErrors = value;
                OnPropertyChanged();
            }
        }

        public double MinThroughput
        {
            get { return _minThroughput; }
            set
            {
                _minThroughput = value;
                OnPropertyChanged();
            }
        }

        public double MaxThroughput
        {
            get { return _maxThroughput; }
            set
            {
                _maxThroughput = value;
                OnPropertyChanged();
            }
        }

        public double AverageThroughput
        {
            get { return _averageThroughput; }
            set
            {
                _averageThroughput = value;
                OnPropertyChanged();
            }
        }

        // Toolbar

        public int NumberOfThreads { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan DeleyBetweenThreadStart { get; set; }

        // Chart

        public PlotModel ChartModel { get; set; }

        #endregion

        public void Cancel()
        {
            if (LoadTest != null)
            {
                LoadTest.Cancel();
            }
        }

        public void Reset()
        {
            if (Heartbeats != null)
            {
                Heartbeats.Clear();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}