using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker;

internal sealed class MonitoringHandler(
    IMessageProducer<MonitoringMessage> monitoringMessagePublisher) : IListenerRegistrationHandler
{
    private readonly IMessageProducer<MonitoringMessage> _monitoringMessagePublisher = monitoringMessagePublisher;

    public string Name => nameof(MonitoringHandler);

    public async Task Handle(IListenerRegistration registration)
    {
        MonitoringMessage message = new(
            new MonitoringMessageMetadata(
                registration.ExchangeName,
                registration.RoutingKey,
                registration.MessageRoute));

        await _monitoringMessagePublisher.ProduceAsync(message);
    }
}