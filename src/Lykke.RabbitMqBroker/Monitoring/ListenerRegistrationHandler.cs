using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class ListenerRegistrationHandler(
    IMessageProducer<MonitoringHeartbeat> monitoringMessagePublisher) : IListenerRegistrationHandler
{
    private readonly IMessageProducer<MonitoringHeartbeat> _monitoringMessagePublisher = monitoringMessagePublisher;

    public string Name => nameof(ListenerRegistrationHandler);

    public async Task Handle(IListenerRegistration registration)
    {
        MonitoringHeartbeat heartbeatMessage = new(
            new MonitoringHeartbeatMetadata(
                registration.ListenerRoute.ExchangeName,
                registration.ListenerRoute.RoutingKey,
                registration.ListenerRoute.ToString()));

        await _monitoringMessagePublisher.ProduceAsync(heartbeatMessage);
    }
}