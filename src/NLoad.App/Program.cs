using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using NLoad.App.Features.RunLoadTest;
using NLoad.App.Tests;

namespace NLoad.App
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();

            var loadTests = GetLoadTests(Assembly.GetExecutingAssembly());

            var viewModel = new LoadTestViewModel(loadTests);

            var window = new LoadTestWindow
            {
                DataContext = viewModel
            };

            app.Run(window);
        }

        private static IEnumerable<Type> GetLoadTests(Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(LoadTestAttribute), true).Length > 0);
        }
    }
}