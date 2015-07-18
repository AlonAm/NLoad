using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    /// <summary>
    /// Interaction logic for LoadTestWindow.xaml
    /// </summary>
    public partial class LoadTestWindow
    {
        public LoadTestWindow()
        {
            InitializeComponent();

            MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
