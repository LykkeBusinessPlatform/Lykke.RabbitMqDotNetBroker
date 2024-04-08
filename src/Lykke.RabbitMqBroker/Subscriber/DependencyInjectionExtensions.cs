// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber
{
    public static class DependencyInjectionExtensions
    {
        public static void AddJsonNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>> configure = null)
        {
            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateNoLossSubscriber(loggerFactory, settings, connection, configure);
                
                if (handler != null)
                    subscriber.Subscribe(handler);
                
                return subscriber;
            });
        }
        
        public static void AddJsonLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>> configure = null)
        {
            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var subscriber = RabbitMqSubscriber<T>
                    .Json
                    .CreateLossAcceptableSubscriber(loggerFactory, settings, connection, configure);
                
                if (handler != null)
                    subscriber.Subscribe(handler);
                
                return subscriber;
            });
        }
        
        public static void AddMessagePackNoLossSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>> configure = null)
        {
            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateNoLossSubscriber(loggerFactory, settings, connection, configure);
                
                if (handler != null)
                    subscriber.Subscribe(handler);
                
                return subscriber;
            });
        }
        
        public static void AddMessagePackLossAcceptableSubscriber<T>(this IServiceCollection services,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection,
            Func<T, Task> handler = null,
            Action<RabbitMqSubscriber<T>> configure = null)
        {
            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var subscriber = RabbitMqSubscriber<T>
                    .MessagePack
                    .CreateLossAcceptableSubscriber(loggerFactory, settings, connection, configure);
                    
                if (handler != null)
                    subscriber.Subscribe(handler);
                
                return subscriber;
            });
        }
    }
}
