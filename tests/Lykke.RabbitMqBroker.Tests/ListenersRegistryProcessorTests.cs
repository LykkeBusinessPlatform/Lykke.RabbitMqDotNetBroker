using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    public class ListenersRegistryProcessorTests
    {
        internal sealed class CountingListenerRegistrationHandler : IListenerRegistrationHandler
        {
            public string Name => "CountingHandler";

            public int Counter { get; private set; } = 0;

            public Task Handle(IListenerRegistration registration)
            {
                Counter++;
                return Task.CompletedTask;
            }
        }

        internal sealed class NameTracingListenerRegistrationHandler : IListenerRegistrationHandler
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

        internal sealed class FailingListenerRegistrationHandler : IListenerRegistrationHandler
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
            var handler = new CountingListenerRegistrationHandler();
            var processor = new ListenersRegistryWorker(
                new List<IListenerRegistrationHandler> { handler },
                NullLogger<ListenersRegistryWorker>.Instance,
                listenersRegistry: null);

            await processor.Execute();

            Assert.That(handler.Counter, Is.EqualTo(0));
        }

        [Test]
        public async Task Handlers_Are_Called_For_Each_Registration()
        {
            var nameTracingHandler = new NameTracingListenerRegistrationHandler();
            var countingHandler = new CountingListenerRegistrationHandler();
            var registration1 = new ListenerRegistration<MessageModel1>("ex1", "q1", "r1");
            var registration2 = new ListenerRegistration<MessageModel2>("ex2", "q2", "r2");
            var processor = new ListenersRegistryWorker(
                new List<IListenerRegistrationHandler> { nameTracingHandler, countingHandler },
                NullLogger<ListenersRegistryWorker>.Instance,
                listenersRegistry: new ListenersRegistry
                {
                    registration1,
                    registration2
                });

            await processor.Execute();

            Assert.That(nameTracingHandler.HandledRegistrations, Contains.Item(registration1.ToString()));
            Assert.That(nameTracingHandler.HandledRegistrations, Contains.Item(registration2.ToString()));
            Assert.That(countingHandler.Counter, Is.EqualTo(2));
        }

        [Test]
        public async Task One_Handler_Failure_Does_Not_Stop_Other_Handlers()
        {
            var failingHandler = new FailingListenerRegistrationHandler();
            var countingHandler = new CountingListenerRegistrationHandler();
            var registration1 = new ListenerRegistration<MessageModel1>("ex1", "q1", "r1");
            var registration2 = new ListenerRegistration<MessageModel2>("ex2", "q2", "r2");
            var processor = new ListenersRegistryWorker(
                new List<IListenerRegistrationHandler> { failingHandler, countingHandler },
                NullLogger<ListenersRegistryWorker>.Instance,
                listenersRegistry: new ListenersRegistry
                {
                    registration1,
                    registration2
                });

            await processor.Execute();

            Assert.That(countingHandler.Counter, Is.EqualTo(2));
        }
    }
}