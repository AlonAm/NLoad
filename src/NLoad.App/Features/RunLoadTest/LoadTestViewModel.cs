using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private int _threadCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            Configuration = new LoadTestConfiguration();
            
            RunLoadTestCommand = new RunLoadTestCommandAsync(this);

            StopLoadTestCommand = new StopLoadTestCommand(this);

            Heartbeats = new List<Heartbeat>();
            
            ChartModel = new LoadTestChart(Heartbeats);

            Defaults();

            Reset();
        }

        #region Properties

        public List<Heartbeat> Heartbeats { get; set; }

        // Commands

        public ICommand RunLoadTestCommand { get; private set; }

        public ICommand StopLoadTestCommand { get; private set; }

        // Display todo: replace with Load Test Result / Heartbeat

        public int ThreadCount
        {
            get { return _threadCount; }
            set
            {
                _threadCount = value;
                OnPropertyChanged();
            }
        }

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

        public LoadTestConfiguration Configuration { get; set; }

        // Chart

        public PlotModel ChartModel { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        #endregion

        public void HandleHeartbeat(Heartbeat heartbeat)
        {
            Heartbeats.Add(heartbeat);

            ThreadCount = heartbeat.ThreadCount;

            Throughput = Math.Round(heartbeat.Throughput, 0, MidpointRounding.AwayFromZero);

            Elapsed = heartbeat.Elapsed.ToTimeString();

            TotalIterations = heartbeat.TotalIterations;

            TotalErrors = heartbeat.TotalErrors;

            ChartModel.InvalidatePlot(true);
        }

        public void HandleLoadTestResult(LoadTestResult result)
        {
            Elapsed = result.TotalRuntime.ToTimeString();

            TotalIterations = result.TotalIterations;

            MinThroughput = result.MinThroughput;

            MaxThroughput = result.MaxThroughput;

            AverageThroughput = result.AverageThroughput;

            TotalErrors = result.TotalErrors;
        }

        public void Reset()
        {
            if (Heartbeats != null)
            {
                Heartbeats.Clear();
            }

            Elapsed = "00:00:00";

            TotalIterations = 0;
            TotalErrors = 0;

            MinThroughput = 0;
            MaxThroughput = 0;
            AverageThroughput = 0;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Defaults()
        {
            Configuration.NumberOfThreads = 10;

            Configuration.Duration = TimeSpan.FromSeconds(30);

            Configuration.DelayBetweenThreadStart = TimeSpan.Zero;
        }
    }
}