using System;

using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber;

/// <summary>
/// Consumer for poison queue to requeue messages back to the original exchange.
/// Not thread safe.
/// </summary>
public class PoisonQueueConsumer
{
    private readonly PoisonQueueConsumerConfigurationOptions _options;
    private IModel _channel;

    public PoisonQueueConsumer(PoisonQueueConsumerConfigurationOptions options)
    {
        _options = options;
    }

    public uint Start(Func<IModel> channelFactory)
    {
        _channel = channelFactory();

        uint processedMessages = 0;
        try
        {
            while (TryRequeueOne()) { processedMessages++; }
        }
        finally
        {
            _channel?.Close();
            _channel.Dispose();
        }

        return processedMessages;
    }

    private bool TryRequeueOne()
    {
        var result = _channel.BasicGet(_options.PoisonQueueName.ToString(), false);
        if (result == null)
        {
            return false;
        }

        try
        {
            _channel.ConfirmSelect();
            _channel.BasicPublish(
                _options.ExchangeName.ToString(),
                _options.RoutingKey.ToString(),
                CreatePropertiesFrom(result.BasicProperties),
                Copy(result.Body));
            _channel.WaitForConfirmsOrDie();
            _channel.BasicAck(result.DeliveryTag, false);
            return true;
        }
        catch (Exception)
        {
            _channel.BasicNack(result.DeliveryTag, false, true);
            throw;
        }
    }

    private IBasicProperties CreatePropertiesFrom(IBasicProperties source)
    {
        IBasicProperties properties = null;

        if (!string.IsNullOrEmpty(_options.RoutingKey.ToString()))
        {
            properties = _channel.CreateBasicProperties();
            properties.Type = _options.RoutingKey.ToString();
        }

        if (source?.Headers?.Count > 0)
        {
            properties ??= _channel.CreateBasicProperties();
            properties.Headers = source.Headers;
        }

        return properties;
    }

    private static byte[] Copy(ReadOnlyMemory<byte> source)
    {
        var copy = new byte[source.Length];
        Buffer.BlockCopy(source.ToArray(), 0, copy, 0, source.Length);
        return copy;
    }
}