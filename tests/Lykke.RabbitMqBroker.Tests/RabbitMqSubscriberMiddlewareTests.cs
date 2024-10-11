using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware;
using Lykke.RabbitMqBroker.Tests.Fakes;

using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class RabbitMqSubscriberMiddlewareTests
{
    class MessageModel { };

    class FakeMiddleware : IEventMiddleware<MessageModel>
    {
        public Task ProcessAsync(IEventContext<MessageModel> context)
        {
            throw new NotImplementedException();
        }
    }

    class AnotherFakeMiddleware : IEventMiddleware<MessageModel>
    {
        public Task ProcessAsync(IEventContext<MessageModel> context)
        {
            throw new NotImplementedException();
        }
    }

    private RabbitMqSubscriber<MessageModel> _subscriber;

    [SetUp]
    public void SetUp()
    {
        _subscriber = new RabbitMqSubscriber<MessageModel>(
            NullLogger<RabbitMqSubscriber<MessageModel>>.Instance,
            new RabbitMqSubscriptionSettings(),
            new FakeConnection());
    }

    [Test]
    public void UseMiddleware_ThrowsArgumentNullException_WhenMiddlewareIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _subscriber.UseMiddleware<FakeMiddleware>(null));
    }

    [Test]
    public void UseMiddleware_ThrowsInvalidOperationException_WhenAlreadyAdded()
    {
        _subscriber.UseMiddleware(new FakeMiddleware());

        Assert.Throws<InvalidOperationException>(() => _subscriber.UseMiddleware(new FakeMiddleware()));
    }

    [Test]
    public void UseMiddlewareAt_ThrowsArgumentNullException_WhenMiddlewareIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _subscriber.UseMiddlewareAt<FakeMiddleware>(0, null));
    }

    [Test]
    public void UseMiddlewareAt_ThrowsInvalidOperationException_WhenAlreadyAdded()
    {
        _subscriber.UseMiddlewareAt(0, new FakeMiddleware());

        Assert.Throws<InvalidOperationException>(() => _subscriber.UseMiddlewareAt(0, new FakeMiddleware()));
    }

    [Test]
    public void UseMiddlewareAt_ThrowsException_WhenIndexIsOutOfRange()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _subscriber.UseMiddlewareAt(1, new FakeMiddleware()));
        Assert.That(ex.Message, Contains.Substring("index"));
    }

    [Test]
    public void UseMiddlewareAt_ThrowsException_WhenIndexIsNegative()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _subscriber.UseMiddlewareAt(-1, new FakeMiddleware()));
        Assert.That(ex.Message, Contains.Substring("index"));
    }

    [Test]
    public void UserMiddlewareAt_AddsMiddlewareAtSpecifiedIndex()
    {
        var middleware = new FakeMiddleware();
        _subscriber.UseMiddleware(middleware);

        var firstMiddlware = new AnotherFakeMiddleware();
        _subscriber.UseMiddlewareAt(0, firstMiddlware);

        Assert.That(_subscriber._middlewareQueue.First(), Is.EqualTo(firstMiddlware));
    }

    [TearDown]
    public void TearDown()
    {
        _subscriber?.Stop();
        _subscriber?.Dispose();
    }
}