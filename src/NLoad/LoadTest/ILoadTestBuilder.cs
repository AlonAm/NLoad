using System;
using System.Threading;

namespace NLoad
{
    public interface ILoadTestBuilder
    {
        ILoadTest Build();

        ILoadTestBuilder OfType(Type type);

        ILoadTestBuilder WithDurationOf(TimeSpan duration);

        ILoadTestBuilder WithNumberOfThreads(int numberOfThreads);

        ILoadTestBuilder WithDeleyBetweenThreadStart(TimeSpan delay);

        ILoadTestBuilder OnHeartbeat(EventHandler<Heartbeat> handler);

        ILoadTestBuilder WithCancellationToken(CancellationToken cancellationToken);
    }
}