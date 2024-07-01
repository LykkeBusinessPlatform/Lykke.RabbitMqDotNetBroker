using System;

using Autofac;

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
    IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>()
        where THandler : class, IMessageHandler<TModel>;
    
    /// <summary>
    /// Add additional message handler to the listener constructed with the provided instance.
    /// </summary>
    /// <param name="handler"></param>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>(THandler handler)
        where THandler : class, IMessageHandler<TModel>;
    
    /// <summary>
    /// Set up listener options if default options are not enough.
    /// </summary>
    /// <param name="setupListenerOptions"></param>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> AddOptions(Action<RabbitMqListenerOptions<TModel>> setupListenerOptions);
    
    /// <summary>
    /// Set up listener options if default options are not enough.
    /// </summary>
    /// <param name="listenerOptions"></param>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> AddOptions(RabbitMqListenerOptions<TModel> listenerOptions) =>
        AddOptions(opt => opt.CopyFrom(listenerOptions));

    /// <summary>
    /// Register listener in DI container as <see cref="IStartable"/> and start it automatically.
    /// </summary>
    /// <returns></returns>
    IRabbitMqListenerRegistrationBuilder<TModel> AutoStart();
}