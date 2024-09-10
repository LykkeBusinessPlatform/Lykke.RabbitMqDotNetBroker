using Newtonsoft.Json;

namespace Lykke.RabbitMqBroker.Monitoring;

record MonitoringMessage(
    [property: JsonIgnore] MonitoringMessageMetadata Metadata,
    string Text = "This is monitoring message");