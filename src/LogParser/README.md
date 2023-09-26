## LogParser

### Purpose

This app extracts rabbitmq events from logs and resends them to their corresponding exchanges.

### Log entry example

```
[2023-02-21 17:47:31Z] [INF] [:Lykke.Snow.Cqrs.Logging.OutgoingMessageLogger:WebHostLogger] -  (||
{
"MessageTypeFullName": "Lykke.MarginTrading.Activities.Contracts.Models.ActivityEvent",
"MessageTypeName": "ActivityEvent",
"Format": 2,
"Message": "k9kgYmNlYWFiNGRhNDYyNDA1M2IwMTU5ZjM0MjFkMzhiNWHX/wD+Y3Bj9QOzmdkgM2M0MDA2MTE3OTg5NGRlYzgzNzEwOTYyOTA3ZmE5YzWoQUEyMDIwMDKgqi02MDM1MDc5NzDX/wD8loBj9QOzBs0XepOoZGVuaW5nMDGoQUEyMDIwMDKqLTYwMzUwNzk3MJA=",
"Headers": {},
"Exchange": "topic://dev.Activities.events.exchange/ActivityEvent",
"RoutingKey": "ActivityEvent",
"Timestamp": "2023-02-21T17:47:31.0046497Z"
}
||)
```

The parser will find the block between (|| and ||) and use the information in json to resend the data.

### Configuration

To configure the app, add *appsettings.json* file to the working directory.

*ConnectionStrings__rabbitmq* - connection string that is used to send events.

*FilterOptions* - determines which events will be send
From, To - filter events by date
ExcludedMessageTypes - filter event by message type name
IncludedMessageTypes - filter event by message type name

Only one message type filter can be active. IncludedMessageTypes takes precedence over ExcludedMessageTypes.
E.g. if IncludedMessageTypes has any values configured, ExcludedMessageTypes will be ignored completely.


*ParsingOptions* - determines parser settings
LogDirectory - path to log files


### How to run

- Build the app (.net6 is required)
- Collect log files with missed events and place them in a directory
- Configure appsettings.json
- The app only logs to console currently, so make sure to save the output 