using Newtonsoft.Json;

namespace Lykke.RabbitMqBroker.Monitoring;

public record MonitoringMessage(
    [property: JsonIgnore] MonitoringMessageMetadata Metadata,
    string Text)
{
    public static MonitoringMessage Create(
        MonitoringMessageMetadata metadata,
        string text = "This is monitoring message") =>
        new(metadata, text);
}
