using System;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Publisher
{
    /// <summary>
    /// General-purpose publisher with delivery tracking 
    /// capabilities.
    /// </summary>
    /// <typeparam name="T">
    /// The message type.
    /// Only required as marker for DI since it is registered
    /// as a singleton per each message type.
    /// </typeparam>
    public interface ITrackableMessagePublisher<T>
    {
        /// <summary>
        /// Publish message with tracking delivery.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="configurator"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <returns>Delivery id</returns>
        Task<Guid> Publish(
            ReadOnlyMemory<byte> body,
            Action<IBasicProperties> configurator = null,
            string exchangeName = null,
            string routingKey = null);
    }
}
