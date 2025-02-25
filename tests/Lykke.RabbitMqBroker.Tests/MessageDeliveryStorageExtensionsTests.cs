using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryStorageExtensionsTests
{
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new MessageDeliveryInMemoryStorage();
    }

    [Test]
    public async Task Add_Creates_New_Message_Delivery_And_Adds_It_To_Storage()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var addedDelivery = await _storage.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(addedDelivery.IsNone, Is.False);
            Assert.That(addedDelivery.Id, Is.EqualTo(id));
        });
    }

    [Test]
    public async Task Add_Creates_New_Message_Delivery_In_Pending_Status()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var addedDelivery = await _storage.Get(id);

        Assert.That(addedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Pending));
    }

    [Test]
    public async Task TrySetDispatched_Returns_False_If_There_Is_No_Message_Delivery()
    {
        var messageDelivery = new MessageDeliveryWithDefaults();

        var updated = await _storage.TrySetDispatched(messageDelivery.Id);

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task TrySetDispatched_Returns_True_If_Message_Delivery_Exists()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var updated = await _storage.TrySetDispatched(id);
        var updatedDelivery = await _storage.Get(id);

        Assert.That(updated);
        Assert.That(updatedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Dispatched));
    }

    [Test]
    public async Task TrySetDispatched_Returns_False_If_Status_Is_Wrong()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var dispatched = await _storage.TrySetDispatched(id);
        var dispatchedAgain = await _storage.TrySetDispatched(id);
        var dispatchedDelivery = await _storage.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(dispatched);
            Assert.That(dispatchedAgain, Is.False);
            Assert.That(dispatchedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Dispatched));
        });
    }

    [Test]
    public async Task TrySetReceived_Returns_False_If_There_Is_No_Message_Delivery()
    {
        var messageDelivery = new MessageDeliveryWithDefaults();

        var updated = await _storage.TrySetReceived(messageDelivery.Id);

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task TrySetReceived_Returns_True_If_Message_Delivery_Exists()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var dispatched = await _storage.TrySetDispatched(id);
        var received = await _storage.TrySetReceived(id);
        var receivedDelivery = await _storage.Get(id);

        Assert.That(dispatched);
        Assert.That(received);
        Assert.That(receivedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Received));
    }

    [Test]
    public async Task TrySetReceived_Returns_False_If_Status_Is_Wrong()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var updated = await _storage.TrySetReceived(id);
        var updatedDelivery = await _storage.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.False);
            Assert.That(updatedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Pending));
        });
    }

    [Test]
    public async Task TrySetFailed_Returns_False_If_There_Is_No_Message_Delivery()
    {
        var messageDelivery = new MessageDeliveryWithDefaults();

        var updated = await _storage.TrySetFailed(messageDelivery.Id, MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None));

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task TrySetFailed_Returns_True_If_Message_Delivery_Exists()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var failed = await _storage.TrySetFailed(id, MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None));
        var failedDelivery = await _storage.Get(id);

        Assert.That(failed);
        Assert.That(failedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Failed));
    }

    [Test]
    public async Task TrySetFailed_Returns_False_When_Failure_Is_Empty()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var updated = await _storage.TrySetFailed(id, MessageDeliveryFailure.Empty);
        var updatedDelivery = await _storage.Get(id);

        Assert.That(updated, Is.False);
        Assert.That(updatedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Pending));
    }

    [Test]
    public async Task TrySetFailed_Returns_False_If_Status_Is_Wrong()
    {
        var id = await _storage.Add(new MessageRouteWithDefaults());
        var failed = await _storage.TrySetFailed(id, MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None));
        var failedAgain = await _storage.TrySetFailed(id, MessageDeliveryFailure.Create(MessageDeliveryFailureReason.BrokerCustodyNotConfirmed));
        var failedDelivery = await _storage.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(failed);
            Assert.That(failedAgain, Is.False);
            Assert.That(failedDelivery.GetStatus, Is.EqualTo(MessageDeliveryStatus.Failed));
        });
    }
}