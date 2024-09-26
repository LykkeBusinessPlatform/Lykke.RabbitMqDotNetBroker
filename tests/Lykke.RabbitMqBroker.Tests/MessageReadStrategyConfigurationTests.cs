using System;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageReadStrategyConfigurationTests
{
    private readonly Func<IModel> _queuePreconditionFailedChannel = () => new QueueDeclarationExpectedFailureFakeChannel();
    private readonly Func<IModel> _queueUnexpectedFailureChannel = () => new QueueDeclarationUnexpectedFailureFakeChannel();
    private readonly Func<IModel> _exchangePreconditionFailedChannel = () => new ExchangeDeclarationExpectedFailureFakeChannel();
    private readonly Func<IModel> _poisonQueuePreconditionFailedChannel = () => new PoisonQueueExpectedFailureFakeChannel();
    private readonly Func<IModel> _queueDeletionFailedChannel = () => new QueueDeletionExpectedFailureFakeChannel();
    private readonly Func<IModel> _allSuccessChannel = () => new PrimitivesConfiguratorFakeChannel();

    [Test]
    public void StrategyTryConfigure_WhenQueuePreconditionFailed_ReturnsFailureImmediately()
    {
        var result = _queuePreconditionFailedChannel.StrategyTryConfigure(
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsFailure);
        Assert.That(result.Error.Code, Is.EqualTo(ConfigurationErrorCode.PreconditionsFailed));
    }

    [Test]
    public void StrategyTryConfigure_WhenUnexpectedFailure_RaisesException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _queueUnexpectedFailureChannel.StrategyTryConfigure(
                new QueueConfigurationOptions(
                    QueueName.Create("q"),
                    ExchangeName.Create("x"),
                    RoutingKey: RoutingKey.Empty)));
    }

    [Test]
    public void StrategyTryConfigure_WhenDlxPreconditionFailed_ReturnsFailure()
    {
        var result = _exchangePreconditionFailedChannel.StrategyTryConfigure(
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsFailure);
        Assert.That(result.Error.Code, Is.EqualTo(ConfigurationErrorCode.PreconditionsFailed));
    }

    [Test]
    public void StrategyTryConfigure_WhenPoisonQueuePreconditionFailed_ReturnsFailure()
    {
        var result = _poisonQueuePreconditionFailedChannel.StrategyTryConfigure(
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsFailure);
        Assert.That(result.Error.Code, Is.EqualTo(ConfigurationErrorCode.PreconditionsFailed));
    }

    [Test]
    public void StrategyTryConfigure_WhenAllSuccess_ReturnsQueueName()
    {
        var queueName = QueueName.Create("q");
        var result = _allSuccessChannel.StrategyTryConfigure(
            new QueueConfigurationOptions(
                queueName,
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsSuccess);
        Assert.That(result.Response, Is.EqualTo(queueName));
    }

    [Test]
    public void StrategyRetryWithQueueRecreation_WhenQueueNotDeleted_RaisesException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _queueDeletionFailedChannel.StrategyRetryWithQueueRecreation(
                new QueueConfigurationOptions(
                    QueueName.Create("q"),
                    ExchangeName.Create("x"),
                    RoutingKey: RoutingKey.Empty)));

        Assert.That(ex.Message, Does.Contain("Failed to delete queue"));
    }

    [Test]
    public void StrategyRetryWithQueueRecreation_WhenQueueDeleted_TriesToConfigureAgain()
    {
        var queueName = QueueName.Create("q");
        var result = _allSuccessChannel.StrategyRetryWithQueueRecreation(
            new QueueConfigurationOptions(
                queueName,
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsSuccess);
        Assert.That(result.Response, Is.EqualTo(queueName));
    }

    [TearDown]
    public void TearDown()
    {
        PrimitivesConfiguratorFakeChannel.ResetCounters();
    }
}