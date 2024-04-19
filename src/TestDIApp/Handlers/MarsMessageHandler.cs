using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class MarsMessageHandler : IMessageHandler<MarsMessage>
{
    private readonly ILogger<MarsMessageHandler> _logger;

    public MarsMessageHandler(ILogger<MarsMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MarsMessage marsMessage)
    {
        var messageJson = JsonSerializer.Serialize(marsMessage);
        _logger.LogInformation("Received message: {message}", messageJson);
        
        return Task.CompletedTask;
    }
}
