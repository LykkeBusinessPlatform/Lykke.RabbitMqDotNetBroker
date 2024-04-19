using System.Text.Json;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using TestDIApp.Messages;

namespace TestDIApp.Handlers;

public class AnotherJupiterMessageHandler : IMessageHandler<JupiterMessage>
{
    private readonly ILogger<AnotherJupiterMessageHandler> _logger;

    public AnotherJupiterMessageHandler(ILogger<AnotherJupiterMessageHandler> logger)
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
