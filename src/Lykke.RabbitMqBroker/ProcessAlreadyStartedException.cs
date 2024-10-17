using System;

namespace Lykke.RabbitMqBroker;

internal sealed class ProcessAlreadyStartedException : Exception
{
    public ProcessAlreadyStartedException(string message) : base(message)
    {
    }
}
