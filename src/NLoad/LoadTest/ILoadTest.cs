namespace NLoad
{
    public interface ILoadTest
    {
        void IncrementIterationsCounter();

        LoadTestResult Run();

        void Cancel();
    }
}