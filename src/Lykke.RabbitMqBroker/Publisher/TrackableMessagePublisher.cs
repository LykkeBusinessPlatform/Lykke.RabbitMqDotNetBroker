using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

using FailureReason = Lykke.RabbitMqBroker.Abstractions.Tracking.MessageDeliveryFailureReason;

namespace Lykke.RabbitMqBroker.Publisher;

public class TrackableMessagePublisher<T> : ITrackableMessagePublisher<T>
{
    private readonly IPurePublisher<T> _purePublisher;
    private readonly IMessageDeliveryStorage _storage;

    public TrackableMessagePublisher(IPurePublisher<T> purePublisher, IMessageDeliveryStorage storage)
    {
        _purePublisher = purePublisher;
        _purePublisher.ReturnedEventHandler += OnReturned;
        _storage = storage;
    }

    private async Task OnReturned(object sender, BasicReturnEventArgs @event)
    {
        var deliveryId = @event.BasicProperties.GetDeliveryId();
        await _storage.TrySetFailed(
            deliveryId,
            MessageDeliveryFailure.Create(FailureReason.Unroutable));
    }

    public async Task<MessageDeliveryId> Publish(
        ReadOnlyMemory<byte> body,
        MessageRoute route,
        Action<IBasicProperties> configurator = null)
    {
        var deliveryId = await _storage.Add(route);

        try
        {
            await _purePublisher.Publish(
                body,
                props => ConfigureProperties(configurator, props, deliveryId),
                route.ExchangeName,
                route.RoutingKey);
        }
        catch (Exception ex)
        {
            await _storage.TrySetFailed(
                deliveryId,
                MessageDeliveryFailure.FromException(ex, GetFailureReason(ex)));
            return deliveryId;
        }

        await _storage.TrySetDispatched(deliveryId);
        return deliveryId;
    }

    private static FailureReason GetFailureReason(Exception ex) => ex switch
    {
        OperationInterruptedException => FailureReason.BrokerCustodyNotConfirmed,
        _ => FailureReason.DispatchError
    };

    private static void ConfigureProperties(Action<IBasicProperties> configurator, IBasicProperties properties, MessageDeliveryId deliveryId)
    {
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers["Host"] = Environment.MachineName;
        properties.SetDeliveryId(deliveryId);

        properties.AppId = new HostAssemblyDisplayName();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        configurator?.Invoke(properties);
    }
}
