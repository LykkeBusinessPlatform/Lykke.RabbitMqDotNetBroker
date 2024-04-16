// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using RabbitMQ.Client;
using TestInvoke.SubscribeExample;

namespace TestInvoke
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            var rabbitMqSettings = new RabbitMqSubscriptionSettings
            {
                QueueName = Environment.GetEnvironmentVariable("RabbitMqQueue"),
                ExchangeName = Environment.GetEnvironmentVariable("RabbitMqExchange"),
                ConnectionString = Environment.GetEnvironmentVariable("RabbitMqConnectionString")
            };

            using var connection = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqSettings.ConnectionString, UriKind.Absolute),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(2)
            }.CreateConnection() as IAutorecoveringConnection;

            HowToSubscribe.CustomSubscriber(rabbitMqSettings, connection);
            HowToSubscribe.Start();

            Console.WriteLine("Working... Press Enter to stop");
            Console.ReadLine();

            Console.WriteLine("Stopping....");
            HowToSubscribe.Stop();
            Console.WriteLine("Stopped");
            Console.ReadLine();
        }


    }
}
