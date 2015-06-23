namespace NLoad
{
    using System;

    public class LoadTestBuilder<T> where T : ITest, new()
    {
        private readonly LoadTest<T> _loadTest = new LoadTest<T>(new LoadTestConfiguration());

        public LoadTest<T> Build()
        {
            return _loadTest;
        }

        public LoadTestBuilder<T> WithNumberOfThreads(int numberOfThreads)
        {
            _loadTest.Configuration.NumberOfThreads = numberOfThreads;

            return this;
        }

        public LoadTestBuilder<T> WithDurationOf(TimeSpan duration)
        {
            _loadTest.Configuration.Duration = duration;

            return this;
        }

        public LoadTestBuilder<T> WithDeleyBetweenThreadStart(TimeSpan delay)
        {
            _loadTest.Configuration.DelayBetweenThreadStart = delay;

            return this;
        }

        public LoadTestBuilder<T> OnCurrentThroughput(EventHandler<double> handler)
        {
            _loadTest.CurrentThroughput += handler;

            return this;
        }
    }
}