using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

public sealed record MessageDeliveryWithDefaults() : MessageDelivery(MessageDeliveryId.Create(), null, null, MessageDeliveryFailure.Empty, MessageRoute.Create(new NonEmptyString("exchange"), new NonEmptyString("queue"), "key"));

public sealed record MessageRouteWithDefaults() : MessageRoute(new NonEmptyString("exchange"), new NonEmptyString("queue"), "key");
