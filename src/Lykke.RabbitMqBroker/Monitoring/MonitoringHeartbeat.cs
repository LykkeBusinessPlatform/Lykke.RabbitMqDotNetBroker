using Newtonsoft.Json;

namespace Lykke.RabbitMqBroker.Monitoring;

record MonitoringHeartbeat(
    [property: JsonIgnore] ListenerRoute Route,
    string Text = "This is monitoring heartbeat message");