using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class PoisonQueueNameTests
{
    [Test]
    public void Create_WhenAlreadyPoisonName_ExpectSameValue()
    {
        const string queueName = "queue-name-poison";
        var result = PoisonQueueName.Create(queueName);

        Assert.That(result.Value, Is.EqualTo(queueName));
    }

    [Test]
    public void Create_Validate_QueueName()
    {
        const string queueName = "queue-name";
        var result = PoisonQueueName.Create(queueName);

        Assert.That(result.Value, Is.EqualTo($"{queueName}-poison"));
    }

    [Test]
    public void FromQueueName_Validate_QueueName()
    {
        const string queueName = "queue-name";
        var result = PoisonQueueName.FromQueueName(QueueName.Create(queueName));

        Assert.That(result.Value, Is.EqualTo($"{queueName}-poison"));
    }
}
