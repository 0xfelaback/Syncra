namespace Syncra.Application.DTOs;

public record SyncRequestDto(string nodeId, List<SyncEventRequest> events, int lastKnownServerSequence);

public record SyncResponseDto(Event.EventStatus status,
 string messageId, DateTime receivedAt, TimeSpan estimatedProcessingTime, string message = "Events queued for processing. Listen for SignalR notifications.");


public record SyncEventRequest(string event_id, int nodeSequence, DateTime nodeTimestamp, Event.EventType eventType, string accountId, EventPayloadDataRequest payload, string? parentEventId);

public record EventPayloadDataRequest
{
    public decimal amount { get; set; }
    public string reason { get; set; } = null!;
    public string to_account_id { get; set; } = null!;
}