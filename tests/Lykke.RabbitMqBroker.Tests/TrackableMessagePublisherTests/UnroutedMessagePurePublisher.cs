using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Tests.TrackableMessagePublisherTests;

internal sealed class UnroutedMessagePurePublisher : IPurePublisher<MessageModel>
{
    public const int IoEmulationDelayMs = 100;
    public event AsyncEventHandler<BasicReturnEventArgs> ReturnedEventHandler;

    public Task Publish(
        ReadOnlyMemory<byte> body,
        Action<IBasicProperties> configurator = null,
        string exchangeName = null,
        string routingKey = null)
    {
        var properties = new FakeProperties();
        configurator?.Invoke(properties);

        // emulate OnReturn on different thread
        Task.Run(async () =>
        {
            // emulate I/O operation
            await Task.Delay(IoEmulationDelayMs);
            ReturnedEventHandler?.Invoke(this, new BasicReturnEventArgs() { BasicProperties = properties });
        });

        return Task.CompletedTask;
    }
}
