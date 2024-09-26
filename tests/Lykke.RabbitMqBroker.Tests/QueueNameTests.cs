using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class QueueNameTests
{
    [Test]
    public void Create_NullValue_RaisesException()
    {
        Assert.That(() => QueueName.Create(null), Throws.ArgumentException);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Create_EmptyValue_RaisesException(string emptyValue)
    {
        Assert.That(() => QueueName.Create(emptyValue), Throws.ArgumentException);
    }

    [Test]
    public void Create_ForbiddenSymbols_RaisesException()
    {
        Assert.That(() => QueueName.Create("name-with-forbidden-symbol-*"), Throws.ArgumentException);
    }

    [Test]
    public void Create_ReservedStrings_RaisesException()
    {
        Assert.That(() => QueueName.Create("amq.-is-reserved-beginning"), Throws.ArgumentException);
    }

    [Test]
    public void Create_TooLongValue_RaisesException()
    {
        Assert.That(() => QueueName.Create(new string('a', 256)), Throws.ArgumentException);
    }

    [Test]
    public void Create_Validate_QueueName()
    {
        const string queueName = "queue-name";
        var result = QueueName.Create(queueName);

        Assert.That(result.Value, Is.EqualTo(queueName));
    }
}