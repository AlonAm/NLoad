namespace NLoad
{
    public interface ILoadTest
    {
        LoadTestResult Run();


        void IncrementTotalIterations();

        void IncrementTotalErrors();
        
        void IncrementTotalThreads();


        LoadTestConfiguration Configuration { get; }

        long TotalIterations { get; }

        long TotalErrors { get; }

        long ThreadCount { get; }
    }
}