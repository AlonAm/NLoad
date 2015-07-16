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

        public LoadTestViewModel()
        {
            Configuration = new LoadTestConfiguration();

            RunLoadTestCommand = new RunLoadTestCommandAsync(this);

            Heartbeats = new List<Heartbeat>();

            ChartModel = new LoadTestChart(Heartbeats);

            Defaults();

            Reset();
        }

        public LoadTestViewModel(IEnumerable<Type> loadTests)
            : this()
        {
            if (loadTests == null)
            {
                throw new ArgumentNullException("loadTests");
            }

            _loadTests = loadTests;

            LoadTest = _loadTests.FirstOrDefault();
        }

        #region Properties

        public List<Heartbeat> Heartbeats { get; set; }

        // Commands

        public ICommand RunLoadTestCommand { get; private set; }

        // Display todo: replace with Load Test Result / Heartbeat

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

        public IEnumerable<Type> LoadTests
        {
            get
            {
                return _loadTests;
            }
        }

        public Type LoadTest { get; set; }

        // Toolbar

        public LoadTestConfiguration Configuration { get; set; }

        // Chart

        public PlotModel ChartModel { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        #endregion

        public void HandleHeartbeat(Heartbeat heartbeat)
        {
            Heartbeat = heartbeat;

            Heartbeats.Add(heartbeat);

            ChartModel.InvalidatePlot(true);
        }

        public void Reset()
        {
            if (Heartbeats != null)
            {
                Heartbeats.Clear();
            }
        }

        private void Defaults()
        {
            Configuration.NumberOfThreads = 5;

            Configuration.Duration = TimeSpan.FromSeconds(30);

            Configuration.DelayBetweenThreadStart = TimeSpan.Zero;
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
}