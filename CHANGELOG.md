## [[tbd]] (2023-09-22)

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
