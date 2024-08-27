using System;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker;

internal sealed class MonitoringMessageSender(
    IMonitoringMessageChannelProvider channelProvider,
    IMonitoringMessagePropertiesFactory propertiesFactory,
    TimeSpan confirmationWaitTimeout) : IListenerRegistrationHandler
{
    private readonly IMonitoringMessageChannelProvider _channelProvider =
        channelProvider ?? throw new ArgumentNullException(nameof(channelProvider));
    private readonly IMonitoringMessagePropertiesFactory _propertiesFactory =
        propertiesFactory ?? throw new ArgumentNullException(nameof(propertiesFactory));
    private readonly TimeSpan _confirmationWaitTimeout = confirmationWaitTimeout;

    public string Name => nameof(MonitoringMessageSender);

    public Task Handle(IListenerRegistration registration)
    {
        using var channel = _channelProvider.Create();
        var properties = _propertiesFactory.Create(channel, registration.MessageRoute);
        channel.BasicPublish(
            exchange: registration.ExchangeName,
            routingKey: registration.RoutingKey ?? string.Empty,
            mandatory: true,
            basicProperties: properties,
            body: ReadOnlyMemory<byte>.Empty
        );
        channel.WaitForConfirmsOrDie(_confirmationWaitTimeout);

        return Task.CompletedTask;
    }
}