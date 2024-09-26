namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

/// <param name="ExchangeName"> The name of the exchange to be declared. </param>
/// <param name="ExchangeType"> The type of the exchange to be declared. </param>
/// <param name="Durable"> Whether the exchange should be durable. </param>
/// <param name="AutoDelete"> Whether the exchange should be auto-deleted. </param>
internal sealed record ExchangeConfigurationOptions(ExchangeName ExchangeName, string ExchangeType, bool Durable, bool AutoDelete);
