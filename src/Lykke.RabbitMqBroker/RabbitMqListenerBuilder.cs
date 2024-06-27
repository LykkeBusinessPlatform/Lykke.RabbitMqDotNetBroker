using System;

using Autofac;

using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.DependencyInjection;

namespace Lykke.RabbitMqBroker;

internal sealed class RabbitMqListenerBuilder<TModel> : IRabbitMqListenerBuilder<TModel> where TModel : class 
{
    public IServiceCollection Services { get; }
        
    public RabbitMqListenerBuilder(IServiceCollection services)
    {
        Services = services;
    }
        
    public IRabbitMqListenerBuilder<TModel> WithAdditionalMessageHandler<THandler>() where THandler : class, IMessageHandler<TModel>
    {
        Services.AddSingleton<IMessageHandler<TModel>, THandler>();
        return this;
    }

    public IRabbitMqListenerBuilder<TModel> WithOptions(Action<RabbitMqListenerOptions<TModel>> setupListenerOptions)
    {
        Services.Configure(setupListenerOptions);
        return this;
    }

    public IRabbitMqListenerBuilder<TModel> AutoStart()
    {
        Services.AddSingleton<IStartable>(p => p.GetService<RabbitMqListener<TModel>>());
        return this;
    }
}