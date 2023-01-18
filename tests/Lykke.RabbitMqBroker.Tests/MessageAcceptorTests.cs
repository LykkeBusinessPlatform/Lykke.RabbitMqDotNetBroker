using Lykke.RabbitMqBroker.Subscriber;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    public class MessageAcceptorTests
    {
        private IModel _channel;
        
        [SetUp]
        public void Setup()
        {
            _channel = Substitute.For<IModel>();
        }
        
        [Test]
        public void Accept_Returns_WhenAlreadyProcessed()
        {
            var sut = new MessageAcceptor(_channel, default);
            
            // accept for the first time
            sut.Accept();
            
            _channel.ReceivedWithAnyArgs().BasicAck(default, default);
            
            _channel.ClearReceivedCalls();
            
            // accept for the second time
            sut.Accept();
            
            _channel.DidNotReceiveWithAnyArgs().BasicAck(default, default);
        }

        [Test]
        public void Reject_Returns_WhenAlreadyProcessed()
        {
            var sut = new MessageAcceptor(_channel, default);
            
            // reject for the first time
            sut.Reject();
            
            _channel.ReceivedWithAnyArgs().BasicReject(default, default);
            
            _channel.ClearReceivedCalls();
            
            // reject for the second time
            sut.Reject();
            
            _channel.DidNotReceiveWithAnyArgs().BasicReject(default, default);
        }
    }
}
