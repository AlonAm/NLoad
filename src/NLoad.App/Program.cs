using System;
using System.Windows;
using NLoad.App.Features.RunLoadTest;

namespace NLoad.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
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

//private static LoadTestWindow CreateWindow()
//{
//    var viewModel = new LoadTestViewModel();

//    var label = new Label
//    {
//    };

//    var textBox = new TextBox();
//    {
//    };

//    var button = new Button
//    {
//        Content = "Run Load Test",
//        Command = new RunLoadTestCommand()
//    };

//    var stackPanel = new StackPanel
//    {
//        Orientation = Orientation.Vertical
//    };

//    stackPanel.Children.Add(label);
//    stackPanel.Children.Add(textBox);
//    stackPanel.Children.Add(button);

//    var binding = new Binding
//    {
//        Source = viewModel,
//        Path = new PropertyPath("Title")
//    };

//    BindingOperations.SetBinding(label, UIElement.VisibilityProperty, binding);

//    var window = new LoadTestWindow
//    {
//        DataContext = viewModel,
//        Content = stackPanel
//    };

//    return window;
//}

//Console Remanings

//private static void RunLoadTest()
//{
//    var loadTest = NLoad.Test<InMemoryTest>()
//                            .WithNumberOfThreads(100)
//                            .WithDurationOf(TimeSpan.FromSeconds(60))
//                            .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
//                            .OnHeartbeat(PrintHeartbeat)
//                            .Build();

//    var result = loadTest.Run();

//    Console.WriteLine("\n [Summary]\n");
//    Console.WriteLine(" Total Runtime: {0}", result.TotalRuntime);
//    Console.WriteLine(" Total TotalIterations: {0}", result.TotalIterations);
//    Console.WriteLine(" Test Runners: {0}", result.TestRunnersResults.Count());
//    Console.WriteLine(" Heartbeats: {0}\n", result.Heartbeat.Count);

//    Console.WriteLine(" [Throughput]\n");
//    Console.WriteLine(" Average: {0}", result.AverageThroughput);
//    Console.WriteLine(" Max    : {0}", result.MaxThroughput);
//    Console.WriteLine(" Min    : {0}\n", result.MinThroughput);

//    Console.WriteLine(" [Response Time]\n");
//    Console.WriteLine(" Average: {0}", result.AverageResponseTime.ToString("c"));
//    Console.WriteLine(" Min    : {0}", result.MinResponseTime.ToString("c"));
//    Console.WriteLine(" Max    : {0}", result.MaxResponseTime.ToString("c"));
//}

//private static void PrintHeartbeat(object sender, Heartbeat e)
//{
//    Console.WriteLine(" {0}  {1}  {2}", e.Timestamp.ToString("T"), e.Elapsed.ToString("c"), e.Throughput);
//}

//private static void Header()
//{
//    Console.WriteLine("\n NLoad \n ________________________________________________\n\n");
//    Console.WriteLine(" Running in memory load test...\n\n");
//    Console.WriteLine(" [Time]       [Elapsed]         [Throughput]\n");
//}

//private static void Footer()
//{
//    Console.WriteLine("\nPress <Enter> to terminate.");
//    Console.ReadLine();
//}