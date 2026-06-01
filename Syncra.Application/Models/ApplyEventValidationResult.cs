public record ApplyEventValidationResult(bool isValid, string reason, decimal preEventBalance, decimal postEventBalance, Event.EventType eventType, long SnapshotUsed, int eventsReplayed);
