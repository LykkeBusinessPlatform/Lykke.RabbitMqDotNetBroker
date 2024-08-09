using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    internal interface IMonitoringMessagePropertiesFactory
    {
        IBasicProperties Create(IModel channel, string messageRoute);
    }
}