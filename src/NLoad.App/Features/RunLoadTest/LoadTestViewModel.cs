using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;
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

        private ILoadTest _loadTest;

        private readonly RunLoadTestCommand _runLoadTestCommand;
        private readonly StopLoadTestCommand _stopLoadTestCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            Defaults();

            _runLoadTestCommand = new RunLoadTestCommand(this);

            _stopLoadTestCommand = new StopLoadTestCommand(this);
        }

        private void Defaults()
        {
            Elapsed = "00:00:00";
            NumberOfThreads = 10;
            Duration = TimeSpan.FromSeconds(30);
            DeleyBetweenThreadStart = TimeSpan.Zero;

            // Throughput Chart

            IterationsPoints = new List<DataPoint>();
            ErrorPoints = new List<DataPoint>();

            PlotModel = new PlotModel
            {
                Background = OxyColors.Transparent,
                PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1)
            };

            PlotModel.Series.Add(new LineSeries
            {
                ItemsSource = IterationsPoints,
                Color = OxyColors.DodgerBlue
            });

            PlotModel.Series.Add(new LineSeries
            {
                ItemsSource = ErrorPoints,
                Color = OxyColors.Red
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

        public ILoadTest LoadTest //todo: the view should not have access to the load-test
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

        public PlotModel PlotModel { get; set; }

        public List<DataPoint> IterationsPoints { get; private set; }

        public List<DataPoint> ErrorPoints { get; private set; }

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