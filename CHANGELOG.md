## 16.1.2 (2024-12-10)

### Fixed
- LT-5967: Fix `x-expires` argument header conversions to use `long` instead `ulong`.

## 16.1.1 (2024-12-09)

### Fixed
- LT-5967: Fix `x-expires` argument header conversions. Fix `overflow` -> `x-overflow` argument usage.

## 16.1.0 (2024-12-06)

### Added
- LT-5967: Introduce queue TTL configuration

## 16.0.0 (2024-10-22)

### Changed
- Upgraded MessagePack library to version 2.5.187, introducing BREAKING CHANGES that required adjustments in how MessagePack is utilized.
- Added asynchronous methods to serilizer and deserializer interfaces.
- Added serializer and desiializer constructors that accept `MessagePackSerializerOptions` instead of `IFormatterResolver`.

This update covers MessagePack's breaking changes and encapsulates them in the library, so the library's users don't have to worry about them. However, if the deserilization was used before with `readStrict` option equal to `true`, the behavior will be different now. The option `readStrict` is not supported anymore, and the deserialization will be NOT strict by default. For strict deserialization implement custom deserializer.

## 15.4.0 (2024-10-21)

### Changed
- LT-5705: Add Cancellation Support and Logging to PoisonQueueHandler

## 15.3.0 (2024-10-18)

### Changed
- LT-5705: Expose exceptions to handle client side

## 15.2.0 (2024-10-18)

### Added
- LT-5705: Add quorum poison queues handler

## 15.1.1 (2024-10-03)

### Fixed
- LT-5705: Poison queue configuration mismath to result in queue deletion and recreation

## 15.1.0 (2024-09-26)

### Changed
- LT-5705: Automatically create new quorum queue instead of classic queue

## 15.0.0 (2024-09-18)

### Added
- LT-5705: Add quorum queues basic support

## 14.0.0 (2024-08-23)

### Changed
- LT-5648: Migrate to .NET 8.0

## 13.10.0 (2024-07-15)

### Added
- LT-5418: ConsumerCount record to cover edge cases 

## 13.9.1 (2024-07-04)

### Changed
- LT-5418: fix a bug in the DI container listener registration extensions

## 13.9.0 (2024-06-28)

### Changed
- LT-5418: Add feature to register provided message handler instance with DI container

## 13.8.0 (2024-06-28)

### Changed
- LT-5418: Improve AddOptions API, ready-to-use options

## 13.7.0 (2024-06-27)

### Changed
- LT-5418: Add DI container registration builder to simplify listener registration and conveniently manage multiple handlers if required.

## 13.6.0 (2024-06-11)

### Changed
- LT-5418: Listeners by default registered as `IStartable`. This can be changed by setting `autoStart` parameter to `false` when registering listener.

## 13.5.0 (2024-06-07)

### Changed
- LT-5418: Connection display name more informative (includes library version)

## 13.4.0 (2024-05-31)

### Fixed
- LT-5418: Add dead letter queue configuration to subscriber template

## 13.3.1 (2024-05-31)

### Changed
- LT-5418: Register connection provider if not registered yet

## 13.3.0 (2024-05-23)

### Changed
- LT-5418: Add Autofac extensions for listener registration

## 13.2.0 (2024-04-22)

### Changed
- LT-5418: Extend listener registration API with service provider

## 13.1.0 (2024-04-22)

### Added
- LT-5418: Listener now supports multiple subscribers

## 13.0.0 (2024-04-22)

### Added
- LT-5418: Listener concept introduced

## 12.6.0 (2024-04-16)

### Added
- LT-5418: Extend API to create subscribers

## 12.5.0 (2024-04-15)

### Changed
- LT-5418: Add connection auto naming to be visible in RabbitMQ dashboard

## 12.4.0 (2024-04-11)

### Changed
- LT-5418: Add handy methods to register subscribers with shared connection

## 12.3.0 (2024-04-11)

### Changed
- LT-5418: Add connection provider to share connections between subscribers

## 12.2.0 (2024-04-10)

### Changed
- LT-5418: Replace subscriber parameter `ILoggerFactory` with `IServiceProvider` to make custom registration flexible

## 12.1.0 (2024-04-10)

### Changed
- LT-5418: Extend subscriber template parameters with `ILoggerFactory` 

## 12.0.0 (2024-04-10)

### Added
- LT-5418: New templates to create fast no loss and loss accepting subscribers 

## 11.12.2 (2023-09-22)

### Changed
- LT-4990: `EventingBasicConsumer` as a baseline for subscribers

## 11.12.1 (2023-09-22)

### Removed
- LT-5001: Remove `RabbitMqSlimPublisher`

## 11.12.0 (2023-09-22)

### Added
- LT-5001: Add `RabbitMqSlimPublisher`

## 11.11.1 (2023-09-21)

### Fixed
- LT-4990: Discard `EventingBasicConsumerNoLock` and use `EventingBasicConsumer` instead

## 11.11.0 (2023-09-19)

### Added
- LT-4990: Add new `EventingBasicConsumerNoLock` to avoid broker pulling

## 11.10.0 (2023-09-19)

### Added
- LT-4989: Add new `QueueingBasicConsumerNoLock` to avoid custom lock when processing incoming messages

## 11.9.4 (2023-08-22)

### Added
- LT-4955: Add new publishing strategy 'PropertiesWithMessageTypeTopicPublishStrategy'

## 11.9.3 (2023-08-10)

### Added
- New nuget publication workflow
