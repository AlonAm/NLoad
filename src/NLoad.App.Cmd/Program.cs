using System;
using System.Threading;

namespace NLoad.App.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Load Test\n");


            var loadTest = NLoad.Test<MyTest>()
                                        .WithNumberOfThreads(10)
                                        .WithDurationOf(TimeSpan.FromSeconds(30))
                                        .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
                                        .OnHeartbeat((s, e) => 
                                            Console.WriteLine("[{0}] {1}", e.Timestamp.ToString("T"), e.Throughput))
                                    .Build();

            var result = loadTest.Run();


            Console.WriteLine("\nTotal Iterations: {0}\n", result.TotalIterations);

            Console.WriteLine("Press <Enter> to terminate.");
            Console.ReadLine();
        }

        class MyTest : ITest
        {
            public void Initialize()
            {
            }

            public TestResult Execute()
            {
                Thread.Sleep(1);

                return new TestResult(true);
            }
        }
    }
}
