using System;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Publisher
{
    /// <summary>
    /// Rabbit MQ general-purpose publisher.
    /// </summary>
    /// <typeparam name="T">
    /// The message type.
    /// Publisher doesn't depend on the message type.
    /// The type is only required as marker for DI since 
    /// it is registered as a singleton per each message type.
    /// </typeparam>
    public interface IPurePublisher<T>
    {
        Task Publish(
            ReadOnlyMemory<byte> body,
            Action<IBasicProperties> configurator = null,
            string exchangeName = null,
            string routingKey = null);
        event AsyncEventHandler<BasicReturnEventArgs> ReturnedEventHandler;
    }
}
