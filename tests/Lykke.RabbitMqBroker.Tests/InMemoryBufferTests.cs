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
        private static object[] _allBufferTypes =
        {
            new object[] { typeof(InMemoryBuffer) },
            new object[] { typeof(LockFreeBuffer) },
            new object[] { typeof(ExperimentalBuffer) }
        };

        private static object[] _noLossBufferTypes =
        {
            new object[] { typeof(InMemoryBuffer) },
            new object[] { typeof(ExperimentalBuffer) }
        };

        [TestCaseSource(nameof(_allBufferTypes))]
        public async Task ShouldWaitForEnqueue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);
            var cts = new CancellationTokenSource();
            var attemptsToRead = 0;

            var thread = new Thread(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        var message = buffer.WaitOneAndPeek(cts.Token);
                        Thread.Sleep(50);
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

            cts.Cancel();

            Assert.That(20, Is.EqualTo(attemptsToRead));
        }

        [TestCaseSource(nameof(_noLossBufferTypes))]
        public void TestEnqueueAndDequeue(Type bufferType)
        {
            var buffer = (IPublisherBuffer)Activator.CreateInstance(bufferType);

            var message = CreateMessage();

            Assert.That(buffer, Is.Not.Null);
            buffer.Enqueue(message, CancellationToken.None);

            Assert.That(1, Is.EqualTo(buffer.Count));

            var peekedMessage = buffer.WaitOneAndPeek(CancellationToken.None);

            Assert.That(message, Is.EqualTo(peekedMessage));
            Assert.That(1, Is.EqualTo(buffer.Count));

            buffer.Dequeue(CancellationToken.None);

            Assert.That(0, Is.EqualTo(buffer.Count));
        }

        [TestCaseSource(nameof(_allBufferTypes))]
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

            Assert.That(itemsCount, Is.EqualTo(buffer.Count));
        }

        [TestCaseSource(nameof(_allBufferTypes))]
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

            Assert.That(itemsCount, Is.EqualTo(counter));
        }

        private static RawMessage CreateMessage(int bodySizeBytes = 100)
        {
            var body = new byte[bodySizeBytes];
            new Random().NextBytes(body);

            return new RawMessage(body, string.Empty, null);
        }
    }
}
