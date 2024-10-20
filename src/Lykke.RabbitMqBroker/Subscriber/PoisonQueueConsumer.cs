using System;
using System.Threading;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<PoisonQueueConsumer> _logger;

    public PoisonQueueConsumer(
        PoisonQueueConsumerConfigurationOptions options,
        ILogger<PoisonQueueConsumer> logger
    )
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public uint Start(Func<IModel> channelFactory) => Start(channelFactory, CancellationToken.None);

    public uint Start(Func<IModel> channelFactory, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(channelFactory);

        using (_channel = channelFactory())
        {
            uint processedMessages = 0;
            while (!cancellationToken.IsCancellationRequested && TryRequeueMessage())
            {
                processedMessages++;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Poison queue messages processing was canceled");
            }

            return processedMessages;
        }
    }

    private bool TryRequeueMessage()
    {
        var result = _channel.BasicGet(_options.PoisonQueueName, false);
        if (result == null)
        {
            return false;
        }

        try
        {
            _channel.ConfirmSelect();
            _channel.BasicPublish(
                _options.ExchangeName,
                _options.RoutingKey,
                CreatePropertiesFrom(result.BasicProperties),
                Copy(result.Body)
            );
            _channel.WaitForConfirmsOrDie();
            _channel.BasicAck(result.DeliveryTag, false);
            return true;
        }
        catch (Exception ex)
        {
            LogRequeueError(result, ex);
            _channel.BasicNack(result.DeliveryTag, false, true);
            throw;
        }
    }

    private void LogRequeueError(BasicGetResult result, Exception ex)
    {
        _logger.LogError(
            ex,
            "Couldn't requeue message with delivery tag {DeliveryTag} from {PoisonQueue} to {Exchange}",
            result.DeliveryTag,
            _options.PoisonQueueName,
            _options.ExchangeName
        );
    }

    private IBasicProperties CreatePropertiesFrom(IBasicProperties source)
    {
        IBasicProperties properties = null;

        if (!string.IsNullOrEmpty(_options.RoutingKey))
        {
            properties = _channel.CreateBasicProperties();
            properties.Type = _options.RoutingKey;
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
        source.Span.CopyTo(copy);
        return copy;
    }
}
