public record PollEventsResponseDto(int currentServerSequence, List<EventPollResponse> events, bool hasMore);

public record EventPollResponse(string eventid, int serverSequence, Event.EventType eventType, string accountId, decimal amount, DateTime serverTimestamp);