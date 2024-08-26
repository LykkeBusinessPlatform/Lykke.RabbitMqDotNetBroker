using System;

using Lykke.RabbitMqBroker.Tests.Fakes;

using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker;

[TestFixture]
internal sealed class MonitoringMessageSenderTests
{
    class MessageModel1 { }

    public void Handle_Publishes_Single_MonitoringMessage()
    {
        var sut = new MonitoringMessageSender(
            new ConnectionProvider(
                NullLogger<ConnectionProvider>.Instance,
                new FakeAutorecoveringConnectionFactory()),
            new MonitoringMessagePropertiesFactory(null),
            null,
            TimeSpan.FromSeconds(1));

        sut.Handle(new ListenerRegistration<MessageModel1>("exchange", "queue"));
    }
}