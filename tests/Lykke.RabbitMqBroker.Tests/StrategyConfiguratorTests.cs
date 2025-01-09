using System;
using System.Collections.Generic;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using Moq;

using NSubstitute;

using NUnit.Framework;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

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
                TimeToLive.Infinite,
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
                TimeToLive.Infinite,
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsSuccess);
    }
    
    [Test]
    public void Configure_WhenQueuePreconditionFailedForQuorumAndOldOneIsClassic_RetriesAndFails()
    {
        var mock = new Mock<IModel>();
        mock.Setup(
                x => x.QueueDeclare(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, Object>>()))
            .Returns<string, bool, bool, bool, IDictionary<string, Object>>(
                (
                    name,
                    _,
                    _,
                    _,
                    _) => new QueueDeclareOk(name, 0, 0));

        mock.Setup(
                x => x.QueueBind(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, Object>>()))
            .Throws(
                new OperationInterruptedException(
                    new ShutdownEventArgs(
                        ShutdownInitiator.Application,
                        Constants.NotFound,
                        "Exchange not found",
                        ""))
            );

        int tryCount = 5;
        var result = StrategyConfigurator.Configure(
            () => mock.Object,
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                TimeToLive.Infinite,
                QueueType: QueueType.Quorum,
                RoutingKey: RoutingKey.Empty),
            1, 1, tryCount);

        mock.Verify(x => x.QueueBind(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IDictionary<string, Object>>()), Times.Exactly(tryCount));
        Assert.That(result.IsFailure);
    }
    
    [Test]
    public void Configure_WhenQueuePreconditionFailedForQuorumAndOldOneIsQuorum_Fails()
    {
        var result = StrategyConfigurator.Configure(
            _queuePreconditionFailedChannel,
            new QueueConfigurationOptions(
                QueueName.Create("q"),
                ExchangeName.Create("x"),
                TimeToLive.Infinite,
                QueueType: QueueType.Quorum,
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
                    TimeToLive.Infinite,
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
                    TimeToLive.Infinite,
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
                TimeToLive.Infinite,
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
                TimeToLive.Infinite,
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
                TimeToLive.Infinite,
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
                TimeToLive.Infinite,
                DeadLetterExchangeName.Create("dlx"),
                RoutingKey: RoutingKey.Empty));

        Assert.That(result.IsSuccess);
        Assert.That(result.Response, Is.EqualTo(queueName));
    }
    
    [Test]
    public void Configure_OnFailure_DoesRetries()
    {
        var queueName = QueueName.Create("q");
        var result = StrategyConfigurator.Configure(
            _allSuccessChannel,
            new QueueConfigurationOptions(
                queueName,
                ExchangeName.Create("x"),
                TimeToLive.Infinite,
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