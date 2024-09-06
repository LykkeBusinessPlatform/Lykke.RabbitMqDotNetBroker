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
        var deliveryId = Guid.Parse(@event.BasicProperties.Headers["DeliveryId"].ToString());
        await _storage.SetFailed(
            deliveryId,
            MessageDeliveryFailure.Create(FailureReason.Unroutable));
    }

    public async Task<Guid> Publish(
        ReadOnlyMemory<byte> body,
        Action<IBasicProperties> configurator = null,
        string exchangeName = null,
        string routingKey = null)
    {
        var deliveryId = await _storage.Add();

        try
        {
            await _purePublisher.Publish(
                body,
                props => ConfigureProperties(configurator, props, deliveryId),
                exchangeName,
                routingKey);
        }
        catch (OperationInterruptedException ex)
        {
            // if publisher confirms enabled
            await _storage.SetFailed(
                deliveryId,
                MessageDeliveryFailure.FromException(ex, FailureReason.BrokerCustodyNotConfirmed));
            return deliveryId;
        }
        catch (Exception ex)
        {
            await _storage.SetFailed(
                deliveryId,
                MessageDeliveryFailure.FromException(ex, FailureReason.DispatchError));
            return deliveryId;
        }

        await _storage.SetDispatched(deliveryId);
        return deliveryId;
    }

    private static void ConfigureProperties(Action<IBasicProperties> configurator, IBasicProperties properties, Guid deliveryId)
    {
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers["Host"] = Environment.MachineName;
        properties.Headers["DeliveryId"] = deliveryId.ToString();

        properties.AppId = new HostAssemblyDisplayName();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        configurator?.Invoke(properties);
    }
}
