using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
internal class CopyTests
{
    [Test]
    public void CopyConstructor_Id_NotChanged()
    {
        var delivery = new MessageDelivery();
        var copy = new MessageDelivery(delivery);

        Assert.That(copy.Id, Is.EqualTo(delivery.Id));
    }
}