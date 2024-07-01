using System;

using Autofac;

using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.Options;

namespace Lykke.RabbitMqBroker;

internal sealed class RabbitMqListenerAutofacContainerRegistrationBuilder<TModel>
    : IRabbitMqListenerRegistrationBuilder<TModel>
    where TModel : class
{
    public RabbitMqListenerAutofacContainerRegistrationBuilder(ContainerBuilder builder)
    {
        Builder = builder;
    }

    public ContainerBuilder Builder { get; }

    public IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>()
        where THandler : class, IMessageHandler<TModel>
    {
        Builder.RegisterType<THandler>()
            .As<IMessageHandler<TModel>>()
            .SingleInstance();
        return this;
    }

    public IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>(THandler handler)
        where THandler : class, IMessageHandler<TModel>
    {
        Builder.RegisterInstance(handler)
            .As<IMessageHandler<TModel>>()
            .SingleInstance();
        return this;
    }

    /// <summary>
    /// Configure listener options if default options are not enough.
    /// Prerequisite: This method depends on "Options" feature so that take
    /// care of registering it with services.AddOptions() before calling this method.
    /// </summary>
    /// <param name="setupListenerOptions"></param>
    /// <returns></returns>
    public IRabbitMqListenerRegistrationBuilder<TModel> AddOptions(
        Action<RabbitMqListenerOptions<TModel>> setupListenerOptions)
    {
        Builder.Register(
                _ => new ConfigureNamedOptions<RabbitMqListenerOptions<TModel>>(string.Empty, setupListenerOptions))
            .As<IConfigureOptions<RabbitMqListenerOptions<TModel>>>()
            .SingleInstance();
        return this;
    }

    public IRabbitMqListenerRegistrationBuilder<TModel> AutoStart()
    {
        Builder.Register(ctx => ctx.Resolve<RabbitMqListener<TModel>>())
            .As<IStartable>();
        return this;
    }
}