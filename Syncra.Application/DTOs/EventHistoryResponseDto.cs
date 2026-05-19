public record EventHistoryResponseDto(string accountId, int totalEvents, int limit, int offset, List<EventHistoryResponse> events);

public record EventHistoryResponse(string eventid, int serverSequence, Event.EventType eventType, decimal amount, string reason, DateTime serverTimestamp, Event.EventStatus status, bool compensated, string compensationEventId);
