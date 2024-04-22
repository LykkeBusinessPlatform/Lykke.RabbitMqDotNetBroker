using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class PlutoMessageHandler : IMessageHandler<PlutoMessage>
{
    private readonly ILogger<PlutoMessageHandler> _logger;

    public PlutoMessageHandler(ILogger<PlutoMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PlutoMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation("Received message: {message}", messageJson);
        
        return Task.CompletedTask;
    }
}
