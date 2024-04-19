using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class MercuryMessageHandler : IMessageHandler<MercuryMessage>
{
    private readonly ILogger<MercuryMessageHandler> _logger;

    public MercuryMessageHandler(ILogger<MercuryMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MercuryMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation("Received message: {message}", messageJson);
        
        return Task.CompletedTask;
    }
}
