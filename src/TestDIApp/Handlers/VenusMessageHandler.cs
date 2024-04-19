using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class VenusMessageHandler : IMessageHandler<VenusMessage>
{
    private readonly ILogger<VenusMessageHandler> _logger;

    public VenusMessageHandler(ILogger<VenusMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(VenusMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation("Received message: {message}", messageJson);
        
        return Task.CompletedTask;
    }
}
