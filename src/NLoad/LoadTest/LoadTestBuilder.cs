using System.Threading;

namespace NLoad
{
    using System;

    public class LoadTestBuilder<T> where T : ITest, new()
    {
        private EventHandler<Heartbeat> _handler;
        private CancellationToken _cancellationToken;
        private readonly LoadTestConfiguration _configuration;

        public LoadTestBuilder()
        {
            _configuration = new LoadTestConfiguration();
        }

        public LoadTestBuilder(LoadTestConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoadTest<T> Build()
        {
            var loadTest = new LoadTest<T>(_configuration, _cancellationToken);

            if (_handler != null)
            {
                loadTest.Heartbeat += _handler;
            }

            return loadTest;
        }

        public LoadTestBuilder<T> WithNumberOfThreads(int numberOfThreads)
        {
            _configuration.NumberOfThreads = numberOfThreads;

            return this;
        }

        public LoadTestBuilder<T> WithDurationOf(TimeSpan duration)
        {
            _configuration.Duration = duration;

            return this;
        }

        public LoadTestBuilder<T> WithDeleyBetweenThreadStart(TimeSpan delay)
        {
            _configuration.DelayBetweenThreadStart = delay;

            return this;
        }

        public LoadTestBuilder<T> OnHeartbeat(EventHandler<Heartbeat> handler)
        {
            _handler = handler;

            return this;
        }

        public LoadTestBuilder<T> WithCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            return this;
        }
    }
}