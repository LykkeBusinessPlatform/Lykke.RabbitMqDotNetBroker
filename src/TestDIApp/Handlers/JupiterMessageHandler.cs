using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class JupiterMessageHandler : IMessageHandler<JupiterMessage>
{
    private readonly ILogger<JupiterMessageHandler> _logger;

    public JupiterMessageHandler(ILogger<JupiterMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(JupiterMessage jupiterMessage)
    {
        var messageJson = JsonSerializer.Serialize(jupiterMessage);
        _logger.LogInformation("Received message: {message}", messageJson);
        
        return Task.CompletedTask;
    }
}
