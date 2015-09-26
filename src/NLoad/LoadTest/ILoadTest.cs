namespace NLoad
{
    public interface ILoadTest
    {
        LoadTestResult Run();

        LoadTestConfiguration Configuration { get; }

        long TotalIterations { get; }

        long TotalErrors { get; }

        long TotalThreads { get; }
    }
}