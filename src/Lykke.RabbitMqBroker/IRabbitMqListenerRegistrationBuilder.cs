using System;

using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// Rabbit MQ listener DI container registration builder.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IRabbitMqListenerRegistrationBuilder<TModel> where TModel : class
{
    /// <summary>
    /// Add additional message handler to the listener.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> WithAdditionalMessageHandler<THandler>()
        where THandler : class, IMessageHandler<TModel>;
        
    /// <summary>
    /// Configure listener options if default options are not enough.
    /// </summary>
    /// <param name="setupListenerOptions"></param>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> WithOptions(Action<RabbitMqListenerOptions<TModel>> setupListenerOptions);
        
    /// <summary>
    /// Register listener in DI container as <see cref="IStartable"/> and start it automatically.
    /// </summary>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> AutoStart();
}