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
        /// Also adds listener registration metadata to the listeners registry, if available,
        /// check <see cref="IListenersRegistry"/>.
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
        /// <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AddMessageHandler{THandler}(THandler)"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="configureSubscriber">Low-level subscriber configuration callback
        /// It is called after the subscriber is created and configured by the library.
        /// Consider it as last chance to configure the subscriber before it starts listening.</param>
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

            services.AddSingleton(p =>
            {
                var registry = p.GetService<IListenersRegistry>();
                registry?.Add(
                    new ListenerRegistration<TModel>(
                        ListenerRoute.Create(
                            new ExchangeName(subscriptionSettings.ExchangeName),
                            new QueueName(subscriptionSettings.QueueName),
                            new RoutingKey(subscriptionSettings.RoutingKey))));

                return new RabbitMqListener<TModel>(
                    p.GetRequiredService<IConnectionProvider>(),
                    subscriptionSettings,
                    p.GetRequiredService<IOptions<RabbitMqListenerOptions<TModel>>>(),
                    s =>
                    {
                        configureSubscriber?.Invoke(s, p);
                        s.UseMonitoringHeartbeatMiddleware(p);
                    },
                    p.GetRequiredService<IEnumerable<IMessageHandler<TModel>>>(),
                    p.GetRequiredService<ILoggerFactory>());
            });

            return new RabbitMqListenerRegistrationBuilder<TModel>(services);
        }

        /// <summary>
        /// Registers a Rabbit MQ listener in the DI container.
        /// Behind the scenes, it registers a low-level subscriber or
        /// multiple subscribers and message handler.
        /// Also adds listener registration metadata to the listeners registry, if available,
        /// check <see cref="IListenersRegistry"/>.
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
        /// <see cref="IRabbitMqListenerRegistrationBuilder{TModel}.AddMessageHandler{THandler}(THandler)"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subscriptionSettings">RabbitMQ host connection settings</param> 
        /// <param name="configureSubscriber">Low-level subscriber configuration callback. 
        /// It is called after the subscriber is created and configured by the library.
        /// Consider it as last chance to configure the subscriber before it starts listening.</param>
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

            builder.Register(ctx =>
                {
                    // the original IComponentContext can't be captured because it is short-lived
                    // and not thread-safe. It will be disposed long before the configure subscriber action
                    // is invoked. So it is required to resolve a new IComponentContext which is LifetimeScope.
                    // see https://autofac.readthedocs.io/en/latest/register/registration.html#lambda-expression-components
                    var registry = ctx.ResolveOptional<IListenersRegistry>();
                    registry?.Add(
                        new ListenerRegistration<TModel>(
                            ListenerRoute.Create(
                                new ExchangeName(subscriptionSettings.ExchangeName),
                                new QueueName(subscriptionSettings.QueueName),
                                new RoutingKey(subscriptionSettings.RoutingKey))));
                    var ccLifetimeScope = ctx.Resolve<IComponentContext>();

                    return new RabbitMqListener<TModel>(
                        ctx.Resolve<IConnectionProvider>(),
                        subscriptionSettings,
                        ctx.Resolve<IOptions<RabbitMqListenerOptions<TModel>>>(),
                        s =>
                        {
                            configureSubscriber?.Invoke(s, ccLifetimeScope);
                            s.UseMonitoringHeartbeatMiddleware(ccLifetimeScope);
                        },
                        ctx.Resolve<IEnumerable<IMessageHandler<TModel>>>(),
                        ctx.Resolve<ILoggerFactory>());
                })
                .AsSelf()
                .SingleInstance();

            return new RabbitMqListenerAutofacContainerRegistrationBuilder<TModel>(builder);
        }
    }
}
