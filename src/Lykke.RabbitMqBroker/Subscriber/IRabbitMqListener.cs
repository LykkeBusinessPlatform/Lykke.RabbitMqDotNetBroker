using System;

namespace Lykke.RabbitMqBroker.Subscriber;

public interface IRabbitMqListener
{
    public bool IsConnected();

    public Type ContractType { get; }
}