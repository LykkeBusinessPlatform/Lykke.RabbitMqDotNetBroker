// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace TestInvoke.SubscribeExample
{
    public static class HowToSubscribe
    {
        private static RabbitMqSubscriber<string> _subscriber;

        public static void Example(RabbitMqSubscriptionSettings settings, IAutorecoveringConnection connection)
        {
            _subscriber =
                new RabbitMqSubscriber<string>(
                    new NullLogger<RabbitMqSubscriber<string>>(),
                    settings,
                    connection)
                    .UseMiddleware(new ExceptionSwallowMiddleware<string>(new NullLogger<ExceptionSwallowMiddleware<string>>()))
                    .SetMessageDeserializer(new TestMessageDeserializer())
                    .UseDefaultStrategy()
                    .Subscribe(HandleMessage);
        }

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
