using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    internal class LoadTestViewModel : INotifyPropertyChanged
    {
        private string _elapsed;
        private long _iterations;
        private double _minThroughput;
        private double _maxThroughput;
        private double _averageThroughput;
        private double _throughput;

        private readonly RunLoadTestCommand _runLoadTestCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            _runLoadTestCommand = new RunLoadTestCommand(this);

            Defaults();
        }

        private void Defaults()
        {
            Elapsed = "00:00:00";
            NumberOfThreads = 10;
            Duration = TimeSpan.FromSeconds(30);
            DeleyBetweenThreadStart = TimeSpan.Zero;
        }

        #region Properties

        public ICommand RunLoadTestCommand
        {
            get { return _runLoadTestCommand; }
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


        public int NumberOfThreads { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan DeleyBetweenThreadStart { get; set; }
        

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