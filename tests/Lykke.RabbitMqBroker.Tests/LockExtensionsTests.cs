using System;
using System.Threading;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class LockExtensionsTests
{
    private SemaphoreSlim _semaphore;

    [SetUp]
    public void SetUp()
    {
        _semaphore = new SemaphoreSlim(1, 1);
    }

    [Test]
    public void Execute_ShouldRunFunction_WhenSemaphoreIsNotLocked()
    {
        var func = new Func<int>(() => 42);

        var result = _semaphore.Execute(func, TimeSpan.FromSeconds(1));

        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Execute_ShouldThrowProcessAlreadyStartedException_WhenSemaphoreIsLocked()
    {
        _semaphore.Wait();

        var func = new Func<int>(() => 42);

        Assert.Throws<ProcessAlreadyStartedException>(() => _semaphore.Execute(func, TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void Execute_ShouldReleaseLock_WhenFunctionIsExecuted()
    {
        var func = new Func<int>(() => 42);

        _semaphore.Execute(func, TimeSpan.FromSeconds(1));

        Assert.That(_semaphore.CurrentCount, Is.EqualTo(1));
    }

    [TearDown]
    public void TearDown()
    {
        _semaphore.Dispose();
    }
}
