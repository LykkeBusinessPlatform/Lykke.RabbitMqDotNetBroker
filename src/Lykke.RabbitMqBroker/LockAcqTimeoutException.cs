using System;

namespace Lykke.RabbitMqBroker;

internal sealed class LockAcqTimeoutException : Exception
{
    public LockAcqTimeoutException(string message) : base(message) { }
}
