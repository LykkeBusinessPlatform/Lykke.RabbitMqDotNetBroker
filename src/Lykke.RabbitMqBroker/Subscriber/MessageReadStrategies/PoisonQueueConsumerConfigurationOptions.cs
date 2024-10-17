using System;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

public sealed record PoisonQueueConsumerConfigurationOptions(
    PoisonQueueName PoisonQueueName,
    ExchangeName ExchangeName,
    RoutingKey RoutingKey)
{
    public static PoisonQueueConsumerConfigurationOptions Create(
        PoisonQueueName poisonQueueName,
        ExchangeName exchangeName,
        RoutingKey routingKey) =>
        new(poisonQueueName, exchangeName, routingKey);
}
