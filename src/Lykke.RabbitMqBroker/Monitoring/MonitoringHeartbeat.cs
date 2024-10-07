using Newtonsoft.Json;

namespace Lykke.RabbitMqBroker.Monitoring;

record MonitoringHeartbeat(
    [property: JsonIgnore] ListenerRoute ListenerRoute,
    string Text = "This is monitoring heartbeat message");