using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
internal sealed class FailureTests
{
    [Test]
    public void Create_WhenCalled_ShouldCreateNewFailure()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);

        Assert.Multiple(() =>
        {
            Assert.That(failure.IsEmpty, Is.False);
            Assert.That(failure.Reason, Is.EqualTo(MessageDeliveryFailureReason.Uncategorised));
        });
    }

    [Test]
    public void FromException_WhenCalled_ShouldCreateFailureFromException()
    {
        var exception = new Exception("Test exception");
        var failure = MessageDeliveryFailure.FromException(exception);

        Assert.Multiple(() =>
        {
            Assert.That(failure.IsEmpty, Is.False);
            Assert.That(failure.Reason, Is.EqualTo(MessageDeliveryFailureReason.Uncategorised));
            Assert.That(failure.Description, Is.EqualTo(exception.Message));
        });
    }
}