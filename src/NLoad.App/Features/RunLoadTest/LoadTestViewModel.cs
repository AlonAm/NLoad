using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    internal class LoadTestViewModel : INotifyPropertyChanged
    {
        private double _currentThroughput;
        private readonly RunLoadTestCommand _runLoadTestCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadTestViewModel()
        {
            _runLoadTestCommand = new RunLoadTestCommand(this);
        }

        public double CurrentThroughput
        {
            get { return _currentThroughput; }
            set
            {
                _currentThroughput = value;
                OnPropertyChanged();
            }
        }

        public ICommand RunLoadTestCommand
        {
            get { return _runLoadTestCommand; }
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