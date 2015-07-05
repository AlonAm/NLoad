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

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            RunLoadTestCommand = new RunLoadTestCommandAsync(this, cancellationTokenSource.Token);
            StopLoadTestCommand = new StopLoadTestCommand(cancellationTokenSource);

            Heartbeats = new List<Heartbeat>();
            ChartModel = new LoadTestChart(Heartbeats);

            Elapsed = "00:00:00";
            NumberOfThreads = 10;
            Duration = TimeSpan.FromSeconds(30);
            DeleyBetweenThreadStart = TimeSpan.Zero;
        }

        #region Properties

        public List<Heartbeat> Heartbeats { get; set; }

        // Commands

        public ICommand RunLoadTestCommand { get; private set; }

        public ICommand StopLoadTestCommand { get; private set; }

        // Display todo: replace with Load Test Result / Heartbeat

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

        public void OnHeartbeat(Heartbeat heartbeat)
        {
            Heartbeats.Add(heartbeat);

            Throughput = Math.Round(heartbeat.Throughput, 0, MidpointRounding.AwayFromZero);

            Elapsed = FormatElapsed(heartbeat.Elapsed);

            TotalIterations = heartbeat.TotalIterations;

            TotalErrors = heartbeat.TotalErrors;

            ChartModel.InvalidatePlot(true);
        }

        public void OnLoadTestCompleted(LoadTestResult result)
        {
            Elapsed = FormatElapsed(result.TotalRuntime);
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
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            return string.Format("{0}:{1}:{2}", elapsed.ToString("hh"), elapsed.ToString("mm"), elapsed.ToString("ss"));
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