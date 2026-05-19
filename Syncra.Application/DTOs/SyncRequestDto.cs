public record SyncRequestDto(string nodeId, List<SyncEventRequest> events, int lastKnownServerSequence);

public record SyncResponseDto(Event.EventStatus status,
 string messageId, DateTime receivedAt, TimeSpan estimatedProcessingTime, string message = "Events queued for processing. Listen for SignalR notifications.");


public record SyncEventRequest(string eventId, int nodeSequence, DateTime nodeTimestamp, Event.EventType eventType, string accountId, Event.EventPayloadData payload, string? parentEventId);