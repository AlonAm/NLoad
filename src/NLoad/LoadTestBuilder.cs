namespace NLoad
{
    using System;

    public class LoadTestBuilder<T> where T : ITestRun, new()
    {
        readonly LoadTestConfiguration _loadTestConfiguration = new LoadTestConfiguration();

        public LoadTest<T> Build()
        {
            return new LoadTest<T>(_loadTestConfiguration);
        }

        public LoadTestBuilder<T> WithNumberOfThreads(int numberOfThreads)
        {
            _loadTestConfiguration.NumberOfThreads = numberOfThreads;

            return this;
        }

        public LoadTestBuilder<T> WithDurationOf(TimeSpan duration)
        {
            _loadTestConfiguration.Duration = duration;

            return this;
        }

        public LoadTestBuilder<T> WithDeleyBetweenThreadStart(TimeSpan delay)
        {
            _loadTestConfiguration.DelayBetweenThreadStart = delay;

            return this;
        }
    }
}