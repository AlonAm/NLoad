namespace NLoad
{
    public interface ILoadTest
    {
        LoadTestResult Run();

        void IncrementIterationsCounter();

        void IncrementErrorsCounter();
        
        void IncrementThreadCount();

        long TotalIterations { get; }

        long TotalErrors { get; }

        LoadTestConfiguration Configuration { get; }
    }
}