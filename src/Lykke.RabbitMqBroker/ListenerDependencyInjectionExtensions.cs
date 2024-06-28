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
        /// Registers a Rabbit MQ listener in the service collection.
        /// Behind the scenes, it registers a low-level subscriber or
        /// multiple subscribers and message handler.
        ///
        /// Listener is not started automatically by default. For this,
        /// use <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AutoStart"/>.
        /// For autostart to work properly, Autofac is required to be used
        /// at least as service provider factory. Otherwise, listener
        /// should be started manually by resolving it from the container
        /// as <see cref="IStartable"/>. If AutoStart not used, then
        /// resolve it as <see cref="RabbitMqListener{TModel}"/>
        /// 
        /// Implements <see cref="IDisposable"/> interface so that container can
        /// take care of disposing the listener when the application stops.
        /// 
        /// Can be registered once for each message type. If required, handling
        /// can be extended by registering more handlers implementing
        /// <see cref="IMessageHandler{TModel}"/> interface either manually or using
        /// <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AddMessageHandler{THandler}"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="configureSubscriber">Low-level subscriber configuration callback</param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns>Rabbit MQ listener registration builder</returns>
        public static IRabbitMqListenerRegistrationBuilder<TModel> AddRabbitMqListener<TModel, THandler>(
            this IServiceCollection services,
            RabbitMqSubscriptionSettings subscriptionSettings,
            Action<RabbitMqSubscriber<TModel>, IServiceProvider> configureSubscriber = null)
            where TModel : class
            where THandler : class, IMessageHandler<TModel>
        {
            services.AddSingleton<IMessageHandler<TModel>, THandler>();
            
            services.AddSingleton(p => new RabbitMqListener<TModel>(
                p.GetRequiredService<IConnectionProvider>(),
                subscriptionSettings,
                p.GetRequiredService<IOptions<RabbitMqListenerOptions<TModel>>>(),
                s => configureSubscriber?.Invoke(s, p),
                p.GetRequiredService<IEnumerable<IMessageHandler<TModel>>>(),
                p.GetRequiredService<ILoggerFactory>()));
            
            return new RabbitMqListenerRegistrationBuilder<TModel>(services);
        }
        
        /// <summary>
        /// Registers a Rabbit MQ listener in the service collection.
        /// Behind the scenes, it registers a low-level subscriber or
        /// multiple subscribers, a message handler and options for the listener.
        /// 
        /// Listener starts subscriber(-s) automatically when the application starts.
        /// For this Autofac is required to be used at least as service provider factory.
        /// Otherwise, listener should be started manually by resolving it
        /// from the container as <see cref="IStartable"/>. If autoStart = false, then
        /// resolve it as <see cref="RabbitMqListener{TModel}"/>
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
        /// <param name="autoStart">If true, listener is registered in DI container as <see cref="IStartable"/>
        /// and starts automatically when Autofac is used as service provider factory</param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        [Obsolete("Please, use the overload which returns IRabbitMqListenerBuilder<TModel>")]
        public static IServiceCollection AddRabbitMqListener<TModel, THandler>(
            this IServiceCollection services,
            RabbitMqSubscriptionSettings subscriptionSettings,
            Action<RabbitMqListenerOptions<TModel>> setupListenerOptions,
            Action<RabbitMqSubscriber<TModel>, IServiceProvider> configureSubscriber = null,
            bool autoStart = true) 
            where TModel : class
            where THandler : class, IMessageHandler<TModel>
        {
            services.AddSingleton<IMessageHandler<TModel>, THandler>();
            
            services.AddSingleton(p => new RabbitMqListener<TModel>(
                p.GetRequiredService<IConnectionProvider>(),
                subscriptionSettings,
                p.GetRequiredService<IOptions<RabbitMqListenerOptions<TModel>>>(),
                s => configureSubscriber?.Invoke(s, p),
                p.GetRequiredService<IEnumerable<IMessageHandler<TModel>>>(),
                p.GetRequiredService<ILoggerFactory>()));

            if (autoStart)
            {
                services.AddSingleton<IStartable>(p => p.GetService<RabbitMqListener<TModel>>());
            }

            services.Configure(setupListenerOptions);
            
            return services;
        }

        /// <summary>
        /// Registers a Rabbit MQ listener in the DI container.
        /// Behind the scenes, it registers a low-level subscriber or
        /// multiple subscribers and message handler.
        ///
        /// Listener is not started automatically by default. For this,
        /// use <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AutoStart"/>.
        /// For autostart to work properly, Autofac is required to be used
        /// at least as service provider factory. Otherwise, listener
        /// should be started manually by resolving it from the container
        /// as <see cref="IStartable"/>. If AutoStart not used, then
        /// resolve it as <see cref="RabbitMqListener{TModel}"/>
        /// 
        /// Implements <see cref="IDisposable"/> interface so that container can
        /// take care of disposing the listener when the application stops.
        /// 
        /// Can be registered once for each message type. If required, handling
        /// can be extended by registering more handlers implementing
        /// <see cref="IMessageHandler{TModel}"/> interface either manually or using
        /// <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AddMessageHandler{THandler}"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="configureSubscriber">Low-level subscriber configuration callback</param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns>Rabbit MQ listener registration builder</returns>
        public static IRabbitMqListenerRegistrationBuilder<TModel> AddRabbitMqListener<TModel, THandler>(
            this ContainerBuilder builder,
            RabbitMqSubscriptionSettings subscriptionSettings,
            Action<RabbitMqSubscriber<TModel>, IComponentContext> configureSubscriber = null)
            where TModel : class
            where THandler : class, IMessageHandler<TModel>
        {
            builder.RegisterType<THandler>()
                .As<IMessageHandler<TModel>>()
                .SingleInstance();

            builder.Register(ctx => new RabbitMqListener<TModel>(
                    ctx.Resolve<IConnectionProvider>(),
                    subscriptionSettings,
                    ctx.Resolve<IOptions<RabbitMqListenerOptions<TModel>>>(),
                    s => configureSubscriber?.Invoke(s, ctx),
                    ctx.Resolve<IEnumerable<IMessageHandler<TModel>>>(),
                    ctx.Resolve<ILoggerFactory>()))
                .AsSelf()
                .SingleInstance();

            return new RabbitMqListenerAutofacContainerRegistrationBuilder<TModel>(builder);
        }


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
        /// 
        /// Prerequisite: This method depends on "Options" feature so that take
        /// care on registering it with services.AddOptions() before calling this method.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="setupListenerOptions">Options which are more business specific than technical</param>
        /// <param name="configureSubscriber">Low-level subscriber configuration callback</param>
        /// <param name="autoStart">If true, listener is registered in DI container as <see cref="IStartable"/>
        /// and starts automatically when Autofac is used as service provider factory</param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        [Obsolete("Please, use the overload which returns IRabbitMqListenerBuilder<TModel>")]
        public static ContainerBuilder AddRabbitMqListener<TModel, THandler>(
            this ContainerBuilder builder,
            RabbitMqSubscriptionSettings subscriptionSettings,
            Action<RabbitMqListenerOptions<TModel>> setupListenerOptions,
            Action<RabbitMqSubscriber<TModel>, IComponentContext> configureSubscriber = null,
            bool autoStart = true)
            where TModel : class
            where THandler : class, IMessageHandler<TModel>
        {
            builder.RegisterType<THandler>()
                .As<IMessageHandler<TModel>>()
                .SingleInstance();

            var listenerRegistration = builder.Register(ctx => new RabbitMqListener<TModel>(
                    ctx.Resolve<IConnectionProvider>(),
                    subscriptionSettings,
                    ctx.Resolve<IOptions<RabbitMqListenerOptions<TModel>>>(),
                    s => configureSubscriber?.Invoke(s, ctx),
                    ctx.Resolve<IEnumerable<IMessageHandler<TModel>>>(),
                    ctx.Resolve<ILoggerFactory>()))
                .AsSelf()
                .SingleInstance();
            
            if (autoStart)
            {
                listenerRegistration.As<IStartable>();
            }

            builder.Register(_ => new ConfigureNamedOptions<RabbitMqListenerOptions<TModel>>(string.Empty, setupListenerOptions))
                .As<IConfigureOptions<RabbitMqListenerOptions<TModel>>>()
                .SingleInstance();

            return builder;
        }
    }
}
