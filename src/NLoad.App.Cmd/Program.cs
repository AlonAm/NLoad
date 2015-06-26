using System;
using System.Linq;

namespace NLoad.App.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Header();

            RunLoadTest();

            Footer();
        }

        private static void RunLoadTest()
        {
            var loadTest = NLoad.Test<InMemoryTest>()
                .WithNumberOfThreads(100)
                .WithDurationOf(TimeSpan.FromSeconds(10))
                .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
                .OnHeartbeat((s, e) =>
                    Console.WriteLine(" {0}  {1}  {2}", e.Timestamp.ToString("T"), e.Elapsed.ToString("c"), e.Throughput))
                .Build();

            var result = loadTest.Run();
            
            Console.WriteLine("\n [Summary]\n");
            Console.WriteLine(" Total Runtime: {0}", result.TotalRuntime);
            Console.WriteLine(" Total Iterations: {0}", result.TotalIterations);
            Console.WriteLine(" Test Runners: {0}", result.TestRunnersResults.Count());
            Console.WriteLine(" Heartbeats: {0}\n", result.Heartbeat.Count);
            
            Console.WriteLine(" [Throughput]\n");
            Console.WriteLine(" Average: {0}", result.AverageThroughput);
            Console.WriteLine(" Max    : {0}", result.MaxThroughput);
            Console.WriteLine(" Min    : {0}\n", result.MinThroughput);
            
            Console.WriteLine(" [Response Time]\n");
            Console.WriteLine(" Average: {0}", result.AverageResponseTime.ToString("g"));
            Console.WriteLine(" Min    : {0}", result.MinResponseTime.ToString("g"));
            Console.WriteLine(" Max    : {0}", result.MaxResponseTime.ToString("g"));
        }

        private static void Header()
        {
            Console.WriteLine("\n NLoad \n ________________________________________________\n\n");
            Console.WriteLine("  Running in memory load test...\n\n");
            Console.WriteLine(" [Time]       [Elapsed]         [Throughput]\n");
        }

        private static void Footer()
        {
            Console.WriteLine("\nPress <Enter> to terminate.");
            Console.ReadLine();
        }
    }
}
