namespace NLoad
{
    public interface ILoadTest
    {
        LoadTestResult Run();


        void IncrementIterationsCounter();

        void IncrementErrorsCounter();
        
        void IncrementThreadCount();


        LoadTestConfiguration Configuration { get; }

        long TotalIterations { get; }

        long TotalErrors { get; }

        long ThreadCount { get; }
    }
}