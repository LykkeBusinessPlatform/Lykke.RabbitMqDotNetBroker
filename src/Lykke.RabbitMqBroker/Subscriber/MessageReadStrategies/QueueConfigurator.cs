using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class QueueConfigurator
{
    public static QueueConfigurationResult Configure(IModel channel, QueueConfigurationOptions options)
    {
        var argumentsBuilder = new QueueDeclarationArgumentsBuilder();
        if (options.ShouldConfigureDeadLettering())
        {
            var deadLetteringConfigurationResult = ConfigureDeadLettering(channel,
                DeadLetteringConfigurationOptions.FromQueueConfigurationOptions(options));
            argumentsBuilder.WithDeadLetterExchange(deadLetteringConfigurationResult.ExchangeName);
        }

        var actualQueueName = channel.QueueDeclare(
            queue: options.QueueName,
            durable: options.Durable,
            exclusive: false,
            autoDelete: options.AutoDelete,
            arguments: argumentsBuilder.Build()).QueueName;

        channel.QueueBind(
            queue: actualQueueName,
            exchange: options.ExchangeName,
            routingKey: options.RoutingKey);

        return new QueueConfigurationResult(actualQueueName);
    }

    private static DeadLetteringConfigurationResult ConfigureDeadLettering(
    IModel channel,
    DeadLetteringConfigurationOptions options)
    {
        channel.ExchangeDeclare(
            exchange: options.ExchangeName,
            type: options.ExchangeType,
            durable: true);
        var actualQueueName = channel.QueueDeclare(
            queue: options.QueueName,
            durable: options.Durable,
            exclusive: false,
            autoDelete: options.AutoDelete).QueueName;
        channel.QueueBind(
            queue: actualQueueName,
            exchange: options.ExchangeName,
            routingKey: options.RoutingKey);

        return new DeadLetteringConfigurationResult(options.ExchangeName);
    }
}
