using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class DeadLetterExchangeNameTests
{
    [Test]
    public void Create_WhenAlreadyDeadLetterName_ExpectSameValue()
    {
        const string exchangeName = "exchange.name.dlx";
        var result = DeadLetterExchangeName.Create(exchangeName);

        Assert.That(result.Value, Is.EqualTo(exchangeName));
    }

    [Test]
    public void Create_Validate_ExchangeName()
    {
        const string exchangeName = "exchange.name";
        var result = DeadLetterExchangeName.Create(exchangeName);

        Assert.That(result.Value, Is.EqualTo($"{exchangeName}.dlx"));
    }

    [Test]
    public void FromExchangeName_Validate_ExchangeName()
    {
        const string exchangeName = "exchange.name";
        var result = DeadLetterExchangeName.FromExchangeName(ExchangeName.Create(exchangeName));

        Assert.That(result.Value, Is.EqualTo($"{exchangeName}.dlx"));
    }
}
