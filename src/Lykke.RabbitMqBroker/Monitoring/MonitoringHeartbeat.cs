using Newtonsoft.Json;

namespace Lykke.RabbitMqBroker.Monitoring;

record MonitoringHeartbeat(
    [property: JsonIgnore] MonitoringHeartbeatMetadata Metadata,
    string Text = "This is monitoring heartbeat message");