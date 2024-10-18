using System;

namespace Lykke.RabbitMqBroker;

public class ProcessAlreadyStartedException : Exception
{
    public ProcessAlreadyStartedException(string message) : base(message)
    {
    }
}
