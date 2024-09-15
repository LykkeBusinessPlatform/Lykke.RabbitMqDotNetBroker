using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MonitoringHandler(
    IMessageProducer<MonitoringHeartbeat> monitoringMessagePublisher) : IListenerRegistrationHandler
{
    private readonly IMessageProducer<MonitoringHeartbeat> _monitoringMessagePublisher = monitoringMessagePublisher;

    public string Name => nameof(MonitoringHandler);

    public async Task Handle(IListenerRegistration registration)
    {
        MonitoringHeartbeat heartbeatMessage = new(
            new MonitoringHeartbeatMetadata(
                registration.ExchangeName,
                registration.RoutingKey,
                registration.MessageRoute));

        await _monitoringMessagePublisher.ProduceAsync(heartbeatMessage);
    }
}