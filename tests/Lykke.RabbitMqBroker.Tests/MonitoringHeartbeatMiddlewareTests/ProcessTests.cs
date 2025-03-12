using System.Collections.Generic;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

[TestFixture]
internal class ProcessTests
{
    private const string MonitoredQueueName = "Q";
    private const string NonMonitoredQueueName = "NotQ";
    private const string DefaultExchangeName = "Ex";
    private const string DefaultRoutingKey = "";

    private MonitoringHeartbeatCountingReceiver _receiver;
    private MonitoringHeartbeatMiddleware<Message> _sut;

    [SetUp]
    public void SetUp()
    {
        _receiver = new MonitoringHeartbeatCountingReceiver();
        _sut = CreateSut(
            _receiver,
            new ListenerRegistration<Message>(CreateListenerRoute()));
    }

    [Test]
    public async Task Process_When_DestinationIsMonitored_Should_Call_Receiver()
    {
        var context = new MonitoringMessageContext(MonitoredQueueName);

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(_receiver.HandleCallsCounter, Is.EqualTo(1));
            Assert.That(context.NextMiddlewareInvoked, Is.False);
        });
    }

    [Test]
    public async Task Process_When_DestinationIsMonitored_Should_Accept()
    {
        var context = new MonitoringMessageContext(MonitoredQueueName);

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.AcceptedCallsCounter, Is.EqualTo(1));
            Assert.That(context.NextMiddlewareInvoked, Is.False);
        });
    }

    [Test]
    public async Task Process_When_DestinationIsNotMonitored_Should_Not_Call_Receiver()
    {
        var context = new MonitoringMessageContext(NonMonitoredQueueName);

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(_receiver.HandleCallsCounter, Is.EqualTo(0));
            Assert.That(context.NextMiddlewareInvoked, Is.False);
        });
    }

    [Test]
    public async Task Process_When_DestinationIsNotMonitored_Should_Accept()
    {
        // this is the case when monitoring message destined to another queue (but same exchage) inevitably comes in,
        // the message should be accepted in order to be removed from the queue
        var context = new MonitoringMessageContext(NonMonitoredQueueName);

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.AcceptedCallsCounter, Is.EqualTo(1));
            Assert.That(context.NextMiddlewareInvoked, Is.False);
        });

    }

    [Test]
    public async Task Process_When_DestinationIsNotMonitored_Should_Not_Be_Propagated()
    {
        // this is the case when monitoring message destined to another queue (but same exchage) inevitably comes in,
        // the message should not be propagated to the next middleware
        var context = new MonitoringMessageContext(NonMonitoredQueueName);

        await _sut.ProcessAsync(context);

        Assert.That(context.NextMiddlewareInvoked, Is.False);
    }

    [Test]
    public async Task Process_When_NotAMonitoringMessage_Should_Not_Call_Receiver()
    {
        var context = new MessageContext();

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(_receiver.HandleCallsCounter, Is.EqualTo(0));
            Assert.That(context.NextMiddlewareInvoked, Is.True);
        });
    }

    [Test]
    public async Task Process_When_NotAMonitoringMessage_Should_Not_Accept()
    {
        var context = new MessageContext();

        await _sut.ProcessAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.AcceptedCallsCounter, Is.EqualTo(0));
            Assert.That(context.NextMiddlewareInvoked, Is.True);
        });
    }

    private static MonitoringHeartbeatMiddleware<Message> CreateSut(
        IMonitoringHeartbeatReceiver receiver,
        params IListenerRegistration[] listenerRegistrations) =>
        new(
            receiver,
            listenerRegistrations.Length == 0
                ? new EmptyListenersRegistry()
                : new NonEmptyListenersRegistry(new List<IListenerRegistration>(listenerRegistrations)));

    private static ListenerRoute CreateListenerRoute(
        string exchangeName = DefaultExchangeName,
        string queueName = MonitoredQueueName,
        string routingKey = DefaultRoutingKey)
    {
        return new ListenerRoute(
            new ExchangeName(exchangeName),
            new QueueName(queueName),
            new RoutingKey(routingKey));
    }
}