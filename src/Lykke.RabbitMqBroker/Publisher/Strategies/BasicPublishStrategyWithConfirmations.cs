using System;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Publisher.Strategies
{
    public abstract class BasicPublishStrategyWithConfirmations : BasicPublishStrategy
    {
        private readonly TimeSpan _defaultConfirmationTimeout = TimeSpan.FromSeconds(5);

        internal BasicPublishStrategyWithConfirmations(RabbitMqSubscriptionSettings settings, string exchangeType)
        : base(settings, exchangeType)
        {
        }

        public override void Configure(IModel channel)
        {
            base.Configure(channel);
            channel.ConfirmSelect();

            channel.BasicNacks += (sender, args) =>
            {
                throw new RabbitMqBrokerException($"Message {args.DeliveryTag} was not acknowledged");
            };
        }

        public override void Publish(IModel channel, RawMessage message)
        {
            base.Publish(channel, message);
            channel.WaitForConfirmsOrDie(Settings.PublisherConfirmationTimeout ?? _defaultConfirmationTimeout);
        }
    }
}
