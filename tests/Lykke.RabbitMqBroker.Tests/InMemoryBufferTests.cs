using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    public class InMemoryBufferTests
    {
        private static readonly object[] AllBufferTypes =
        [
            new object[] { typeof(InMemoryBuffer) },
            new object[] { typeof(LockFreeBuffer) },
            new object[] { typeof(ExperimentalBuffer) }
        ];

        private static readonly object[] NoLossBufferTypes =
        [
            new object[] { typeof(InMemoryBuffer) },
            new object[] { typeof(ExperimentalBuffer) }
        ];

        [TestCaseSource(nameof(AllBufferTypes))]
        public async Task ShouldWaitForEnqueue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);
            var cts = new CancellationTokenSource();
            var attemptsToRead = 0;

            var thread = new Thread(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        var message = buffer.WaitOneAndPeek(cts.Token);
                        await Task.Delay(50);
                        attemptsToRead++;
                        if (message != null)
                        {
                            buffer.Dequeue(cts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        //that's ok )
                    }
                }
            });

            thread.Start();

            var writeTasks = Enumerable.Range(0, 10).Select(i =>
                Task.Factory.StartNew(() =>
                {
                    buffer.Enqueue(new RawMessage(new byte[0], string.Empty, null), cts.Token);
                    buffer.Enqueue(new RawMessage(new byte[0], string.Empty, null), cts.Token);
                })
            );

            await Task.WhenAll(writeTasks);
            await Task.Delay(TimeSpan.FromSeconds(2));

            await cts.CancelAsync();
            cts.Dispose();

            Assert.That(attemptsToRead, Is.EqualTo(20));
        }

        [TestCaseSource(nameof(NoLossBufferTypes))]
        public void TestEnqueueAndDequeue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);

            var message = CreateMessage();

            Assert.That(buffer, Is.Not.Null);
            buffer.Enqueue(message, CancellationToken.None);

            Assert.That(buffer, Has.Count.EqualTo(1));

            var peekedMessage = buffer.WaitOneAndPeek(CancellationToken.None);

            Assert.That(message, Is.EqualTo(peekedMessage));
            Assert.That(buffer, Has.Count.EqualTo(1));

            buffer.Dequeue(CancellationToken.None);

            Assert.That(buffer, Is.Empty);
        }

        [TestCaseSource(nameof(AllBufferTypes))]
        public void TestConcurrentEnqueue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);
            Assert.That(buffer, Is.Not.Null);
            const int itemsCount = 1000;

            var tasks = Enumerable.Range(0, itemsCount).Select(i =>
                Task.Run(() =>
                {
                    var message = CreateMessage();
                    buffer.Enqueue(message, CancellationToken.None);
                }));

            Task.WaitAll(tasks.ToArray());

            Assert.That(buffer, Has.Count.EqualTo(itemsCount));
        }

        [TestCaseSource(nameof(AllBufferTypes))]
        public void TestSingleThreadedDequeue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);
            Assert.That(buffer, Is.Not.Null);

            const int itemsCount = 1000;

            for (var i = 0; i < itemsCount; i++)
            {
                var message = CreateMessage();
                buffer.Enqueue(message, CancellationToken.None);
            }

            var counter = 0;
            while (buffer.Count > 0)
            {
                var message = buffer.WaitOneAndPeek(CancellationToken.None);
                Assert.That(message, Is.Not.Null);
                buffer.Dequeue(CancellationToken.None);
                counter++;
            }

            Assert.That(counter, Is.EqualTo(itemsCount));
        }

        private static RawMessage CreateMessage(int bodySizeBytes = 100)
        {
            var body = new byte[bodySizeBytes];
            new Random().NextBytes(body);

            return new RawMessage(body, string.Empty, null);
        }
    }
}
