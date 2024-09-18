using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MonitoringHeartbeatReceiverTests
{
    class FakeStorage : IMessageDeliveryStorage
    {
        private Dictionary<MessageDeliveryId, MessageDeliveryStatus> _received { get; set; } = [];

        public IReadOnlyList<MessageDeliveryId> Received => _received
            .Where(x => x.Value == MessageDeliveryStatus.Received)
            .Select(x => x.Key)
            .ToList();

        public Task<bool> AddOrUpdate(MessageDelivery messageDelivery)
        {
            _received.Add(messageDelivery.Id, messageDelivery.GetStatus());
            return Task.FromResult(true);
        }

        public Task<MessageDelivery> Get(MessageDeliveryId id) =>
            Task.FromResult(MessageDelivery.Create(id).TrySetDispatched(DateTime.UtcNow));
    }

    private FakeStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new FakeStorage();
    }

    [Test]
    public async Task Handle_MarksDeliveryAsReceived()
    {
        var receiver = new MonitoringHeartbeatReceiver(_storage);
        var deliveryId = MessageDeliveryId.Create();

        await receiver.Handle(Array.Empty<byte>(), deliveryId);

        Assert.That(_storage.Received, Contains.Item(deliveryId));
    }
}