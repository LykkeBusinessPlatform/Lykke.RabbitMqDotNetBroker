using System;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class StrategyConfiguratorTests
{
    private readonly Func<IModel> _queuePreconditionFailedChannel = () => new QueueDeclarationExpectedSingleFailureFakeChannel();
    private readonly Func<IModel> _queueUnexpectedFailureChannel = () => new QueueDeclarationUnexpectedFailureFakeChannel();
    private readonly Func<IModel> _exchangePreconditionFailedChannel = () => new ExchangeDeclarationExpectedFailureFakeChannel();
    private readonly Func<IModel> _poisonQueuePreconditionFailedChannel = () => new PoisonQueueExpectedFailureFakeChannel();
    private readonly Func<IModel> _queueDeletionFailedChannel = () => new QueueDeletionExpectedFailureFakeChannel();
    private readonly Func<IModel> _allSuccessChannel = () => new PrimitivesConfiguratorFakeChannel();

    [Test]
    public void Configure_WhenQueuePreconditionFailed_DeletesQueueOnce()
    {
        var queueName = QueueName.Create("q");
        StrategyConfigurator.Configure(
            _queuePreconditionFailedChannel,
            new QueueConfigurationOptions(
                queueName,
                ExchangeName.Create("x"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(QueueDeclarationExpectedSingleFailureFakeChannel.DeletedQueues, Has.One.EqualTo(queueName.ToString()));
    }

    [Test]
    public void Configure_WhenQueuePreconditionFailed_Retries()
    {
        var result = StrategyConfigurator.Configure(
            _queuePreconditionFailedChannel,
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsSuccess);
    }

    [Test]
    public void Configure_WhenQueueNotDeleted_RaisesException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            StrategyConfigurator.Configure(
                _queueDeletionFailedChannel,
                new QueueConfigurationOptions(
                    QueueName.Create("q"),
                    ExchangeName.Create("x"),
                    RoutingKey: RoutingKey.Empty)));

        Assert.That(ex.Message, Does.Contain("Failed to delete queue"));
    }

    [Test]
    public void Configure_WhenUnexpectedFailure_RaisesException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            StrategyConfigurator.Configure(
                _queueUnexpectedFailureChannel,
                new QueueConfigurationOptions(
                    QueueName.Create("q"),
                    ExchangeName.Create("x"),
                    RoutingKey: RoutingKey.Empty)));
    }

    [Test]
    public void Configure_WhenDlxPreconditionFailed_ReturnsFailure()
    {
        var result = StrategyConfigurator.Configure(
            _exchangePreconditionFailedChannel,
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsFailure);
        Assert.That(result.Error.Code, Is.EqualTo(ConfigurationErrorCode.PreconditionsFailed));
    }

    [Test]
    public void Configure_WhenPoisonQueuePreconditionFailed_ReturnsFailure()
    {
        var result = StrategyConfigurator.Configure(
            _poisonQueuePreconditionFailedChannel,
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsFailure);
        Assert.That(result.Error.Code, Is.EqualTo(ConfigurationErrorCode.PreconditionsFailed));
    }

    [Test]
    public void Configure_WhenPoisonQueuePreconditionFailed_DeletesQueue()
    {
        var queueName = QueueName.Create("q");
        StrategyConfigurator.Configure(
            _poisonQueuePreconditionFailedChannel,
            new QueueConfigurationOptions(
                queueName,
                ExchangeName.Create("x"),
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(PoisonQueueExpectedFailureFakeChannel.DeletedQueues, Has.One.EqualTo(queueName.AsPoison().ToString()));
    }

    [Test]
    public void Configure_WhenAllSuccess_ReturnsQueueName()
    {
        var queueName = QueueName.Create("q");
        var result = StrategyConfigurator.Configure(
            _allSuccessChannel,
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
        PoisonQueueExpectedFailureFakeChannel.ResetCounters();
        QueueDeclarationExpectedSingleFailureFakeChannel.ResetCounters();
    }
}