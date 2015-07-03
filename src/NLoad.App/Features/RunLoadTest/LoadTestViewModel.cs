using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;

namespace NLoad.App.Features.RunLoadTest
{
    public class LoadTestViewModel : INotifyPropertyChanged
    {
        private string _elapsed;
        private long _iterations;
        private double _minThroughput;
        private double _maxThroughput;
        private double _averageThroughput;
        private double _throughput;

        private readonly RunLoadTestCommand _runLoadTestCommand;
        private readonly StopLoadTestCommand _stopLoadTestCommand;
        private ILoadTest _loadTest;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            Defaults();

            var worker = new BackgroundWorker();

            _runLoadTestCommand = new RunLoadTestCommand(this, worker);
            
            _stopLoadTestCommand = new StopLoadTestCommand(this, worker);
        }

        private void Defaults()
        {
            Elapsed = "00:00:00";
            NumberOfThreads = 10;
            Duration = TimeSpan.FromSeconds(30);
            DeleyBetweenThreadStart = TimeSpan.Zero;
            
            Points = new List<DataPoint>();
            PlotModel = new PlotModel();
            
            PlotModel.Series.Add(new LineSeries()
            {
                ItemsSource = Points
            });
        }

        #region Properties

        // Commands

        public ICommand RunLoadTestCommand
        {
            get { return _runLoadTestCommand; }
        }

        public ICommand StopLoadTestCommand
        {
            get { return _stopLoadTestCommand; }
        }

        // Display

        //todo: the view should not have access to the load-test
        public ILoadTest LoadTest
        {
            get { return _loadTest; }
            set
            {
                _loadTest = value;
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

        public long Iterations
        {
            get { return _iterations; }
            set
            {
                _iterations = value;
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

        public PlotModel PlotModel { get; set; }

        public List<DataPoint> Points { get; private set; }

        #endregion

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