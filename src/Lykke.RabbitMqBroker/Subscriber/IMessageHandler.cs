using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Subscriber
{
    public interface IMessageHandler<in T>
    {
        public Task Handle(T message);
    }
}
