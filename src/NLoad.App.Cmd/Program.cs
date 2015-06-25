using System;
using System.Threading;

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
                .WithNumberOfThreads(10)
                .WithDurationOf(TimeSpan.FromSeconds(30))
                .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
                .OnHeartbeat((s, e) =>
                    Console.WriteLine(" {0}  {1}  {2}", e.Timestamp.ToString("T"), e.Elapsed.ToString("c"), e.Throughput))
                .Build();

            var result = loadTest.Run();

            Console.WriteLine("\nTotal Iterations: {0}\n", result.TotalIterations);
        }

        private static void Header()
        {
            Console.WriteLine("\n  NLoad \n ________________________________________________\n\n");
            Console.WriteLine(" Running in memory load test...\n\n");
            Console.WriteLine(" [Time]       [Elapsed]         [Throughput]\n");
        }

        private static void Footer()
        {
            Console.WriteLine("Press <Enter> to terminate.");
            Console.ReadLine();
        }

        private sealed class InMemoryTest : ITest
        {
            public void Initialize()
            {
                Console.WriteLine("Initialize test...");
            }

            public TestResult Execute()
            {
                Thread.Sleep(1);

                return new TestResult(true);
            }
        }
    }
}
