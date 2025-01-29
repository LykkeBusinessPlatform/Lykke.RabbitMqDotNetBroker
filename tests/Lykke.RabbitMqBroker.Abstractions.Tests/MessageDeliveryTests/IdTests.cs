using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
internal sealed class IdTests
{
    [Test]
    public void Create_WhenCalled_ShouldCreateNewId()
    {
        var id = MessageDeliveryId.Create();

        Assert.That(id.IsEmpty, Is.False);
    }

    [Test]
    public void Parse_WhenCalled_ShouldParseId()
    {
        var guid = Guid.NewGuid();
        var id = MessageDeliveryId.Parse(guid.ToString());

        Assert.Multiple(() =>
        {
            Assert.That(id.IsEmpty, Is.False);
            Assert.That(id.Value, Is.EqualTo(guid));
        });
    }

    [Test]
    public void Parse_WhenCalledWithNonGuidString_ShouldReturnEmptyId()
    {
        var id = MessageDeliveryId.Parse("non-guid");

        Assert.That(id.IsEmpty);
    }

    [Test]
    public void EmptyInstance_ShouldBeEmpty()
    {
        var id = MessageDeliveryId.Empty;

        Assert.That(id.Value, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnGuid()
    {
        var id = MessageDeliveryId.Create();
        var idStr = id.ToString();
        var expected = id.Value.ToString();

        Assert.That(expected, Is.EqualTo(idStr));
    }
}