// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Autofac;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lykke.RabbitMqBroker
{
    public static class ListenerDependencyInjectionExtensions
    {
        /// <summary>
        /// Registers a Rabbit MQ listener in the DI container.
        /// Behind the scenes, it registers a low-level subscriber or
        /// multiple subscribers, a message handler and options for the listener.
        ///
        /// Listener starts subscriber(-s) automatically when the application starts.
        /// For this Autofac is required to be used at least as service provider factory.
        /// Otherwise, listener should be started manually by resolving it
        /// from the container as <see cref="IStartable"/>.
        ///
        /// Implements <see cref="IDisposable"/> interface so that container can
        /// take care of disposing the listener when the application stops.
        ///
        /// Can be registered once for each message type. If required, handling
        /// can be extended by registering more handlers implementing
        /// <see cref="IMessageHandler{TModel}"/> interface.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="setupListenerOptions">Options which are more business specific than technical</param>
        /// <param name="configureSubscriber">Low-level subscriber configuration callback</param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqListener<TModel, THandler>(
            this IServiceCollection services,
            RabbitMqSubscriptionSettings subscriptionSettings,
            Action<RabbitMqListenerOptions<TModel>> setupListenerOptions,
            Action<RabbitMqSubscriber<TModel>> configureSubscriber = null) 
            where TModel : class
            where THandler : class, IMessageHandler<TModel>
        {
            services.AddSingleton<IMessageHandler<TModel>, THandler>();
            
            services.AddSingleton(p => new RabbitMqListener<TModel>(
                p.GetRequiredService<IConnectionProvider>(),
                subscriptionSettings,
                p.GetRequiredService<IOptions<RabbitMqListenerOptions<TModel>>>(),
                configureSubscriber,
                p.GetRequiredService<IEnumerable<IMessageHandler<TModel>>>(),
                p.GetRequiredService<ILoggerFactory>()));

            services.AddSingleton<IStartable>(p => p.GetService<RabbitMqListener<TModel>>());
            
            services.Configure(setupListenerOptions);
            
            return services;
        }
    }
}
