namespace NLoad
{
    public interface ILoadTest
    {
        void IncrementIterationsCounter();
        void IncrementErrorsCounter();

        LoadTestResult Run();

        void Cancel();
    }
}