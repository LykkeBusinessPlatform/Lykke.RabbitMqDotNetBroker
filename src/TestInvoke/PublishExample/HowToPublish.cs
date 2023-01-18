// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace TestInvoke.PublishExample
{
    public static class HowToPublish
    {
        public static void Example(RabbitMqSubscriptionSettings settings, IAutorecoveringConnection connection)
        {
            var publisher = new RabbitMqPublisher<string>(new NullLoggerFactory(), settings, connection)
                .SetSerializer(new TestMessageSerializer())
                .SetPublishStrategy(new FanoutPublishStrategy(settings));
            publisher.Start();

            for (var i = 0; i <= 10; i++)
                publisher.ProduceAsync("message#" + i);
        }
    }
}
