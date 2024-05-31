## [[tbd]] (2024-05-31)

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
