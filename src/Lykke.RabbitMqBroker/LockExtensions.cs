using System;
using System.Threading;

namespace Lykke.RabbitMqBroker;

internal static class LockExtensions
{
    public static T Execute<T>(this SemaphoreSlim semaphore, Func<T> func, TimeSpan waitTimeout)
    {
        if (semaphore.CurrentCount == 0)
        {
            throw new ProcessAlreadyStartedException("The lock has already been acquired");
        }

        if (!semaphore.Wait(waitTimeout))
        {
            throw new LockAcqTimeoutException($"Failed to acquire lock within the specified timeout: {waitTimeout}");
        }

        try
        {
            return func();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
