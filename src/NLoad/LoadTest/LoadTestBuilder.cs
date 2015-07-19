using System.Threading;

namespace NLoad
{
    using System;

    public class LoadTestBuilder : ILoadTestBuilder
    {
        private Type _testType;
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

        public ILoadTest Build()
        {
            var loadTest = new LoadTest(_testType, _configuration, _cancellationToken);

            if (_handler != null)
            {
                loadTest.Heartbeat += _handler;
            }

            return loadTest;
        }

        public ILoadTestBuilder WithNumberOfThreads(int numberOfThreads)
        {
            _configuration.NumberOfThreads = numberOfThreads;

            return this;
        }

        public ILoadTestBuilder WithDurationOf(TimeSpan duration)
        {
            _configuration.Duration = duration;

            return this;
        }

        public ILoadTestBuilder WithDeleyBetweenThreadStart(TimeSpan delay)
        {
            _configuration.DelayBetweenThreadStart = delay;

            return this;
        }

        public ILoadTestBuilder OnHeartbeat(EventHandler<Heartbeat> handler)
        {
            _handler = handler;

            return this;
        }

        public ILoadTestBuilder WithCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            return this;
        }

        public ILoadTestBuilder OfType(Type testType)
        {
            _testType = testType;

            return this;
        }
    }
}