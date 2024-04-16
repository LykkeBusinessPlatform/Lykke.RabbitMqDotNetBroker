// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace TestInvoke.SubscribeExample
{
    public static class HowToSubscribe
    {
        private static RabbitMqSubscriber<string> _subscriber;

        #region Create custom suscriber
        public static void CustomSubscriber(RabbitMqSubscriptionSettings settings, IAutorecoveringConnection connection)
        {
            _subscriber =
                new RabbitMqSubscriber<string>(
                        new NullLogger<RabbitMqSubscriber<string>>(),
                        settings,
                        connection)
                    .UseMiddleware(
                        new ExceptionSwallowMiddleware<string>(new NullLogger<ExceptionSwallowMiddleware<string>>()))
                    .SetMessageDeserializer(new TestMessageDeserializer())
                    .UseDefaultStrategy()
                    .Subscribe(HandleMessage);
        }
        #endregion

        #region Create subscriber manually with handy methods when no need to add to DI container
        public static void Create_NoLoss_MessagePack_Subscriber(
            RabbitMqSubscriptionSettings settings, 
            IAutorecoveringConnection connection)
        {
            _subscriber = RabbitMqSubscriber<string>
                .MessagePack
                .CreateNoLossSubscriber(
                    settings,
                    connection,
                    NullLoggerFactory.Instance)
                .Subscribe(HandleMessage);
        }

        public static void Create_NoLoss_MessagePack_Subscriber_With_ServiceLocator(
            IServiceProvider provider,
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection)
        {
            _subscriber = RabbitMqSubscriber<string>
                .MessagePack
                .CreateNoLossSubscriber(
                    provider,
                    settings,
                    connection)
                .Subscribe(HandleMessage);
        }

        public static void Create_LossAcceptable_Json_Subscriber(
            RabbitMqSubscriptionSettings settings, 
            IAutorecoveringConnection connection)
        {
            _subscriber = RabbitMqSubscriber<string>
                .Json
                .CreateLossAcceptableSubscriber(
                    settings,
                    connection,
                    NullLoggerFactory.Instance)
                .Subscribe(HandleMessage);
        }
        
        public static void Create_LossAcceptable_Json_Subscriber_With_ServiceLocator(
            IServiceProvider provider,
            RabbitMqSubscriptionSettings settings, 
            IAutorecoveringConnection connection)
        {
            _subscriber = RabbitMqSubscriber<string>
                .Json
                .CreateLossAcceptableSubscriber(
                    provider,
                    settings,
                    connection)
                .Subscribe(HandleMessage);
        }
        
        #endregion

        #region Create subcriber in semi-automatic mode and inject
        public static void Inject_NoLoss_MessagePack_Subscriber_With_Shared_Connection_Auto_Resolve(
            IServiceCollection services, 
            RabbitMqSubscriptionSettings settings)
        {
            services.AddMessagePackNoLossSubscriber<string>(settings, HandleMessage);
        }
        
        public static void Inject_NoLoss_MessagePack_Subscriber_With_Custom_Connection(
            IServiceCollection services, 
            RabbitMqSubscriptionSettings settings,
            IAutorecoveringConnection connection)
        {
            services.AddMessagePackNoLossSubscriber<string>(settings, connection, HandleMessage);
        }
        
        #endregion
        
        public static void Start()
        {
            _subscriber.Start();
        }

        public static void Stop()
        {
            _subscriber.Stop();
        }

        private static Task HandleMessage(string msg)
        {
            Console.WriteLine(msg);
            return Task.FromResult(0);
        }
    }
}
