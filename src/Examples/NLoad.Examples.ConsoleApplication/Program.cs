using System;
using System.Threading;

namespace NLoad.Examples.ConsoleApplication
{
    class Program
    {
        static void Main()
        {
            var loadTest = NLoad.Test<MyTest>()
                                    .WithNumberOfThreads(10)
                                    .WithDurationOf(TimeSpan.FromSeconds(10))
                                    .OnHeartbeat((s, e) => Console.WriteLine(e.Throughput))
                                .Build();

            var result = loadTest.Run();

            Console.WriteLine("Total Iterations: {0}", result.TotalIterations);
            Console.WriteLine("\nTotal Runtime: {0}", result.TotalRuntime);
            Console.WriteLine("Average Throughput: {0}", result.AverageThroughput);
            Console.WriteLine("Average Response Time: {0}", result.AverageResponseTime);

            Console.WriteLine("\nPress <Enter> to terminate.");
            Console.ReadLine();
        }
    }

    /// Test Implementation
    class MyTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1000);

            return TestResult.Successful;
        }
    }
}
