using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker;

public class PoisonQueueHandler : IPoisonQueueHandler
{
    private readonly string _connectionString;
    private readonly IConnectionProvider _connectionProvider;
    private readonly PoisonQueueConsumerConfigurationOptions _options;
    private uint _initialMessagesCount;
    private uint _messagesRequeued;

    public PoisonQueueHandler(
        string connectionString,
        IConnectionProvider connectionProvider,
        PoisonQueueConsumerConfigurationOptions options)
    {
        _connectionString = connectionString;
        _connectionProvider = connectionProvider;
        _options = options;
    }

    public string TryPutMessagesBack()
    {
        using var connection = _connectionProvider.GetExclusive(_connectionString);
        using var channel = connection.CreateModel();

        EnsureResourcesExistOrThrow(channel, out _initialMessagesCount);

        _messagesRequeued = _initialMessagesCount switch
        {
            > 0 => new PoisonQueueConsumer(_options).Start(connection.CreateModel),
            _ => default
        };

        return BuildResultText();
    }

    private void EnsureResourcesExistOrThrow(IModel channel, out uint messagesCount)
    {
        messagesCount = channel.QueueDeclarePassive(_options.PoisonQueueName.ToString()).MessageCount;
        channel.ExchangeDeclarePassive(_options.ExchangeName.ToString());
    }

    private string BuildResultText() => _messagesRequeued switch
    {
        0 => string.Empty,
        _ => $"Messages requeue finished. Initial number of messages {_initialMessagesCount}. Processed number of messages {_messagesRequeued}"
    };
}