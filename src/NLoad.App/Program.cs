using System;
using System.Windows;
using NLoad.App.Features.RunLoadTest;

namespace NLoad.App
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();

            var viewModel = new LoadTestViewModel();
            
            var window = new LoadTestWindow
            {
                DataContext = viewModel
            };

            app.Run(window);
        }
    }
}