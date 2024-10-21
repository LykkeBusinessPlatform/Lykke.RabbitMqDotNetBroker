using System;
using System.Threading;

namespace Lykke.RabbitMqBroker;

public class ParallelExecutionGuardPoisonQueueDecorator : IPoisonQueueHandler
{
    private readonly IPoisonQueueHandler _decoratee;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(10);

    public ParallelExecutionGuardPoisonQueueDecorator(IPoisonQueueHandler decoratee)
    {
        _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
    }

    public string TryPutMessagesBack(CancellationToken cancellationToken = default) =>
        _lock.Execute(() => _decoratee.TryPutMessagesBack(cancellationToken), _timeout);
}
