using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware;

using Moq;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class MessageContext : IEventContext<Message>
{
    private readonly Mock<IBasicProperties> _basicPropertiesMock = new();

    public MessageContext(bool isMonitoringMessage = false, string desinationQueueName = "")
    {
        _basicPropertiesMock
            .Setup(m => m.Type)
            .Returns(isMonitoringMessage ? ServiceMessageType.Monitoring.ToString() : string.Empty);

        var headers = new Dictionary<string, object> { { MonitoringMessageHeaders.DeliveryIdHeader, Guid.Empty } };

        if (!string.IsNullOrEmpty(desinationQueueName))
        {
            headers.Add(
                MonitoringMessageHeaders.DestinationQueueHeader,
                desinationQueueName);
        }

        _basicPropertiesMock
            .Setup(m => m.Headers)
            .Returns(headers);
    }

    public ReadOnlyMemory<byte> Body => Array.Empty<byte>();

    public IBasicProperties BasicProperties { get => _basicPropertiesMock.Object; }

    public IMessageAcceptor MessageAcceptor { get; } = new CountingMessageAcceptor();

    public RabbitMqSubscriptionSettings Settings => throw new NotImplementedException();

    public CancellationToken CancellationToken => throw new NotImplementedException();

    public uint AcceptedCallsCounter => ((CountingMessageAcceptor)MessageAcceptor).AcceptCallsCounter;

    public uint RejectedCallsCounter => ((CountingMessageAcceptor)MessageAcceptor).RejectCallsCounter;

    public bool NextMiddlewareInvoked { get; private set; } = false;

    public Task InvokeNextAsync()
    {
        NextMiddlewareInvoked = true;
        return Task.CompletedTask;
    }
}