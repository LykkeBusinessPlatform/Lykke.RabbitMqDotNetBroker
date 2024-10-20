using System;
using System.Threading;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// Requees messages from poison queue back to its original exchange.
/// Checks if poison queue and exchange exists.
/// Not thread safe.
/// </summary>
public class PoisonQueueHandler : IPoisonQueueHandler
{
    private readonly string _connectionString;
    private readonly IConnectionProvider _connectionProvider;
    private readonly PoisonQueueConsumerConfigurationOptions _options;
    private readonly ILogger<PoisonQueueConsumer> _consumerLogger;
    private uint _initialMessagesCount;
    private uint _messagesRequeued;

    public PoisonQueueHandler(
        string connectionString,
        IConnectionProvider connectionProvider,
        PoisonQueueConsumerConfigurationOptions options,
        ILoggerFactory loggerFactory
    )
    {
        _connectionString = string.IsNullOrEmpty(connectionString)
            ? throw new ArgumentNullException(nameof(connectionString))
            : connectionString;
        _connectionProvider =
            connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _consumerLogger =
            loggerFactory?.CreateLogger<PoisonQueueConsumer>()
            ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public string TryPutMessagesBack(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionProvider.GetExclusive(_connectionString);
        using var channel = connection.CreateModel();

        EnsureResourcesExistOrThrow(channel, out _initialMessagesCount);

        _messagesRequeued = _initialMessagesCount switch
        {
            > 0 => CreateConsumer().Start(connection.CreateModel, cancellationToken),
            _ => 0,
        };

        return BuildResultText();
    }

    private PoisonQueueConsumer CreateConsumer() => new(_options, _consumerLogger);

    private void EnsureResourcesExistOrThrow(IModel channel, out uint messagesCount)
    {
        try
        {
            messagesCount = channel.QueueDeclarePassive(_options.PoisonQueueName).MessageCount;
            channel.ExchangeDeclarePassive(_options.ExchangeName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Required queue [{_options.PoisonQueueName}] or exchange [{_options.ExchangeName}] does not exist",
                ex
            );
        }
    }

    private string BuildResultText() =>
        _messagesRequeued switch
        {
            0 => string.Empty,
            _ => $"Messages requeue finished. Initial number of messages {_initialMessagesCount}. Processed number of messages {_messagesRequeued}",
        };
}
