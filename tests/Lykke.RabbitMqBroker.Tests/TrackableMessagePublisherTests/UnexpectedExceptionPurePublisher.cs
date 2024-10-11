using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Tests.TrackableMessagePublisherTests;

internal sealed class UnexpectedExceptionPurePublisher : IPurePublisher<MessageModel>
{
    public event AsyncEventHandler<BasicReturnEventArgs> ReturnedEventHandler;

    public Task Publish(
        ReadOnlyMemory<byte> body,
        Action<IBasicProperties> configurator = null,
        string exchangeName = null,
        string routingKey = null)
    {
        throw new Exception();
    }
}
