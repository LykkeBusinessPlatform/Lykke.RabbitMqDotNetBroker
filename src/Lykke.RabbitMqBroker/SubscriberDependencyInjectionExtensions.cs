// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Autofac;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    public static class SubscriberDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a JSON subscriber to the service collection with no loss guarantee.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="connection"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddJsonNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateNoLossSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a JSON subscriber to the service collection with no loss guarantee.
        /// Uses shared connection.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddJsonNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var connectionProvider = p.GetRequiredService<IConnectionProvider>();
                var connection = connectionProvider.GetOrCreateShared(settings.ConnectionString);

                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateNoLossSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a JSON subscriber to the service collection with loss acceptable guarantee.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="connection"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddJsonLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateLossAcceptableSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a JSON subscriber to the service collection with loss acceptable guarantee.
        /// Uses shared connection.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddJsonLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var connectionProvider = p.GetRequiredService<IConnectionProvider>();
                var connection = connectionProvider.GetOrCreateShared(settings.ConnectionString);

                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateLossAcceptableSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a MessagePack subscriber to the service collection with no loss guarantee.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="connection"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddMessagePackNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateNoLossSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a MessagePack subscriber to the service collection with no loss guarantee.
        /// Uses shared connection.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddMessagePackNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var connectionProvider = p.GetRequiredService<IConnectionProvider>();
                var connection = connectionProvider.GetOrCreateShared(settings.ConnectionString);

                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateNoLossSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a MessagePack subscriber to the service collection with loss acceptable guarantee.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="connection"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddMessagePackLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateLossAcceptableSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a MessagePack subscriber to the service collection with loss acceptable guarantee.
        /// Uses shared connection.
        /// Resolve <see cref="RabbitMqSubscriber{T}"/> to get the instance.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddMessagePackLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>, IServiceProvider> configure = null)
        {
            services.AddSingleton(p =>
            {
                var connectionProvider = p.GetRequiredService<IConnectionProvider>();
                var connection = connectionProvider.GetOrCreateShared(settings.ConnectionString);

                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateLossAcceptableSubscriber(p, settings, connection, configure);

                if (handler != null)
                    subscriber.Subscribe(handler);

                return subscriber;
            });
        }

        /// <summary>
        /// Adds a connection provider to the service collection.
        /// Available as <see cref="IConnectionProvider"/>.
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMqConnectionProvider(this IServiceCollection services)
        {
            services.AddSingleton<IAutorecoveringConnectionFactory, AutorecoveringConnectionFactory>();
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();
        }
        
        /// <summary>
        /// Adds a connection provider to the container
        /// Available as <see cref="IConnectionProvider"/>.
        /// </summary>
        /// <param name="builder"></param>
        public static void AddRabbitMqConnectionProvider(this ContainerBuilder builder)
        {
            builder.RegisterType<AutorecoveringConnectionFactory>()
                .As<IAutorecoveringConnectionFactory>()
                .SingleInstance();
            builder.RegisterType<ConnectionProvider>()
                .As<IConnectionProvider>()
                .SingleInstance();
        }
    }
}
