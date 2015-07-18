using System.Windows.Data;
using System.Windows.Documents;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    public class LoadTestViewModel : INotifyPropertyChanged
    {
        private readonly IEnumerable<Type> _loadTests;
        private LoadTestResult _loadTestResult;
        private Heartbeat _lastHeartbeat;
        private string _runButtonText;
        private bool _isRunning;

        public LoadTestViewModel()
        {
            Configuration = new LoadTestConfiguration();

            RunLoadTestCommand = new RunLoadTestCommand(this);

            Heartbeats = new List<Heartbeat>();

            ChartModel = new LoadTestChart(Heartbeats);

            Defaults();
        }

        public LoadTestViewModel(IEnumerable<Type> loadTests)
            : this()
        {
            if (loadTests == null)
            {
                throw new ArgumentNullException("loadTests");
            }

            _loadTests = loadTests;

            SelectedLoadTest = _loadTests.FirstOrDefault();
        }

        #region Properties

        public List<Heartbeat> Heartbeats { get; set; }

        // Commands

        public ICommand RunLoadTestCommand { get; private set; }

        // UI

        public LoadTestResult LoadTestResult
        {
            get { return _loadTestResult; }
            set
            {
                _loadTestResult = value;
                OnPropertyChanged();
            }
        }

        public Heartbeat Heartbeat
        {
            get { return _lastHeartbeat; }
            set
            {
                _lastHeartbeat = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Type> LoadTests { get { return _loadTests; } }

        public Type SelectedLoadTest { get; set; }

        public string RunButtonText
        {
            get { return _runButtonText; }
            set
            {
                _runButtonText = value;
                OnPropertyChanged();
            }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public LoadTestConfiguration Configuration { get; set; }

        // Chart

        public PlotModel ChartModel { get; set; }

        #endregion

        public void HandleHeartbeat(Heartbeat heartbeat)
        {
            Heartbeat = heartbeat;

            Heartbeats.Add(heartbeat);

            ChartModel.InvalidatePlot(true);
        }

        private void Defaults()
        {
            Configuration.NumberOfThreads = 5;
            Configuration.Duration = TimeSpan.FromSeconds(30);
            Configuration.DelayBetweenThreadStart = TimeSpan.Zero;

            RunButtonText = "Run";
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}