using System;

using NUnit.Framework;

using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageConfirmationFailureHandlerTests;

[TestFixture]
internal sealed class HandleTests
{
    [Test]
    public void Handles_UnConfirmed_Message_And_Raises_Exception()
    {
        var sut = new MonitoringMessageConfirmationFailureHandler(new UnconfirmedMessageEmulator());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue")));
        Assert.That(ex.InnerException, Is.TypeOf<OperationInterruptedException>());
    }

    [Test]
    public void Handles_Only_UnConfirmed_Message()
    {
        var sut = new MonitoringMessageConfirmationFailureHandler(new ArgumentExceptionEmulator());

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue")));
        Assert.That(ex.InnerException, Is.Null);
    }

    [Test]
    public void When_Message_Confirmed_Successfully_Then_No_Exceptions()
    {
        var sut = new MonitoringMessageConfirmationFailureHandler(new SuccessEmulator());

        Assert.DoesNotThrowAsync(async () => await sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue")));
    }
}