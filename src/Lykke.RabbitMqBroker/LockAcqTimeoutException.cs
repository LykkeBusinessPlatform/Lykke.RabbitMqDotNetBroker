using System;

namespace Lykke.RabbitMqBroker;

public class LockAcqTimeoutException : Exception
{
    public LockAcqTimeoutException(string message) : base(message) { }
}
