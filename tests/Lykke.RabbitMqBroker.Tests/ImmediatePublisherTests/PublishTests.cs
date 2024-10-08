using System;

using Lykke.RabbitMqBroker.Publisher;

using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.ImmediatePublisherTests;


[TestFixture]
internal class PublishTests
{
    private FakeConnectionProvider _connectionProvider;

    [SetUp]
    public void SetUp()
    {
        _connectionProvider = new FakeConnectionProvider();
    }

    [Test]
    public void Publish_Creates_New_Connection()
    {
        var publisher = new ImmediatePublisher<MessageModel>(
            _connectionProvider,
            Options.Create(new RabbitMqPublisherOptions<MessageModel>()),
            new RabbitMqSubscriptionSettings());

        publisher.Publish(new ReadOnlyMemory<byte>());

        var totalConnectionsCount = _connectionProvider.ExclusiveConnectionCount + _connectionProvider.SharedConnectionCount;
        Assert.That(totalConnectionsCount, Is.EqualTo(1));
    }

    [Test]
    public void Publish_When_PublisherConfirmesEnabled_Creates_Channel_With_Confirmation()
    {
        var publisher = new ImmediatePublisher<MessageModel>(
            _connectionProvider,
            Options.Create(new RabbitMqPublisherOptions<MessageModel> { PublisherConfirmsEnabled = true }),
            new RabbitMqSubscriptionSettings());

        publisher.Publish(new ReadOnlyMemory<byte>());

        Assert.That(_connectionProvider.LatestConnection.LatestChannel.PublisherConfirms);
    }

    [TearDown]
    public void TearDown()
    {
        _connectionProvider.Dispose();
    }
}