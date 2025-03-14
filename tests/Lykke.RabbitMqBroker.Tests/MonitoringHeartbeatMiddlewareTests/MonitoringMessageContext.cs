namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class MonitoringMessageContext : MessageContext
{
    public MonitoringMessageContext(string destinationQueueName) : base(true, destinationQueueName)
    {
    }
}