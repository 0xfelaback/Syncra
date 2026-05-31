using Syncra.Application.DTOs;

public class EntryService : IEntryService
{
    private readonly IEventRepository _eventRepository;
    public EntryService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    public async Task<List<Event>> SaveBatchRequests(string node_id, List<SyncEventRequest> events, CancellationToken cancellationToken)
    {
        List<Event> eventsList = [];
        foreach (var item in events)
        {
            Event transactionEvent = new Event
            {
                event_id = item.event_id,
                parent_event_id = item.parentEventId,
                aggregateId = item.accountId,
                node_id = node_id,
                node_sequence = item.nodeSequence,
                node_timestamp = item.nodeTimestamp,
                server_sequence = null,
                Type = item.eventType,
                server_timestamp = null,
                payload = new Event.EventPayloadData
                {
                    amount = item.payload.amount,
                    reason = item.payload.reason,
                    from_account_id = item.accountId,
                    to_account_id = item.payload.to_account_id
                },
                Status = Event.EventStatus.Pending,
            };
            eventsList.Append(transactionEvent);
        }
        await _eventRepository.AddCollectionOfEvents(eventsList);
        int saved = await _eventRepository.SaveChangesAsync(cancellationToken);
        return eventsList;

    }
}


public interface IEntryService
{
    Task<List<Event>> SaveBatchRequests(string node_id, List<SyncEventRequest> events, CancellationToken cancellationToken);
}