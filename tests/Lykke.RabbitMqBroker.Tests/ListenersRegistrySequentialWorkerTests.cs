using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal class ListenersRegistrySequentialWorkerTests
{
    internal sealed class FakeCountingListenerRegistrationHandler : IListenerRegistrationHandler
    {
        public string Name => "CountingHandler";

        public int Counter { get; private set; } = 0;

        public Task Handle(IListenerRegistration registration)
        {
            Counter++;
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeNameTracingListenerRegistrationHandler : IListenerRegistrationHandler
    {
        public string Name => "NameTracingHandler";
        private readonly HashSet<string> _handledRegistrations = new();
        public IReadOnlyCollection<string> HandledRegistrations { get => _handledRegistrations.ToList(); }

        public Task Handle(IListenerRegistration registration)
        {
            _handledRegistrations.Add(registration.ToString());
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeFailingListenerRegistrationHandler : IListenerRegistrationHandler
    {
        public string Name => "FailingHandler";

        public Task Handle(IListenerRegistration registration)
        {
            throw new InvalidOperationException($"FailingHandler exception on purpose for registration: {registration}");
        }
    }

    class MessageModel1 { }
    class MessageModel2 { }

    [Test]
    public async Task When_ListenersRegistry_NotProvided_SkipsHandling()
    {
        var handler = new FakeCountingListenerRegistrationHandler();
        var worker = new ListenersRegistrySequentialWorker(
            [handler],
            NullLogger<ListenersRegistrySequentialWorker>.Instance,
            listenersRegistry: null);

        await worker.Execute();

        Assert.That(handler.Counter, Is.EqualTo(0));
    }

    [Test]
    public async Task Handlers_Are_Called_For_Each_Registration()
    {
        var nameTracingHandler = new FakeNameTracingListenerRegistrationHandler();
        var countingHandler = new FakeCountingListenerRegistrationHandler();
        var registration1 = new ListenerRegistration<MessageModel1>(ListenerRoute.Create(new ExchangeName("ex1"), new QueueName("q1"), new RoutingKey("r1")));
        var registration2 = new ListenerRegistration<MessageModel2>(ListenerRoute.Create(new ExchangeName("ex2"), new QueueName("q2"), new RoutingKey("r2")));
        var worker = new ListenersRegistrySequentialWorker(
            [nameTracingHandler, countingHandler],
            NullLogger<ListenersRegistrySequentialWorker>.Instance,
            listenersRegistry: new ListenersRegistry
            {
                registration1,
                registration2
            });

        await worker.Execute();

        Assert.That(nameTracingHandler.HandledRegistrations, Contains.Item(registration1.ToString()));
        Assert.That(nameTracingHandler.HandledRegistrations, Contains.Item(registration2.ToString()));
        Assert.That(countingHandler.Counter, Is.EqualTo(2));
    }

    [Test]
    public async Task One_Handler_Failure_Does_Not_Stop_Other_Handlers()
    {
        var failingHandler = new FakeFailingListenerRegistrationHandler();
        var countingHandler = new FakeCountingListenerRegistrationHandler();
        var registration1 = new ListenerRegistration<MessageModel1>(ListenerRoute.Create(new ExchangeName("ex1"), new QueueName("q1"), new RoutingKey("r1")));
        var registration2 = new ListenerRegistration<MessageModel2>(ListenerRoute.Create(new ExchangeName("ex2"), new QueueName("q2"), new RoutingKey("r2")));
        var worker = new ListenersRegistrySequentialWorker(
            [failingHandler, countingHandler],
            NullLogger<ListenersRegistrySequentialWorker>.Instance,
            listenersRegistry: new ListenersRegistry
            {
                registration1,
                registration2
            });

        await worker.Execute();

        Assert.That(countingHandler.Counter, Is.EqualTo(2));
    }
}