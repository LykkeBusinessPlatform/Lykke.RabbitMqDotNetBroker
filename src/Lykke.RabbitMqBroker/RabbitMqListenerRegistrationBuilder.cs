using System;

using Autofac;

using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.DependencyInjection;

namespace Lykke.RabbitMqBroker;

internal sealed class RabbitMqListenerRegistrationBuilder<TModel> : IRabbitMqListenerRegistrationBuilder<TModel>
    where TModel : class
{
    public RabbitMqListenerRegistrationBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>()
        where THandler : class, IMessageHandler<TModel>
    {
        Services.AddSingleton<IMessageHandler<TModel>, THandler>();
        return this;
    }

    public IRabbitMqListenerRegistrationBuilder<TModel> AddMessageHandler<THandler>(THandler handler)
        where THandler : class, IMessageHandler<TModel>
    {
        Services.AddSingleton<IMessageHandler<TModel>>(handler);
        return this;
    }

    public IRabbitMqListenerRegistrationBuilder<TModel> AddOptions(
        Action<RabbitMqListenerOptions<TModel>> setupListenerOptions)
    {
        Services.Configure(setupListenerOptions);
        return this;
    }

    public IRabbitMqListenerRegistrationBuilder<TModel> AutoStart()
    {
        Services.AddSingleton<IStartable>(p => p.GetService<RabbitMqListener<TModel>>());
        return this;
    }
}