namespace Lykke.RabbitMqBroker;

public record ListenerRegistration<TModel>(ListenerRoute ListenerRoute) : IListenerRegistration;
