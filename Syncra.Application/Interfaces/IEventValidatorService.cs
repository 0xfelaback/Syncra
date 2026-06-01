namespace Syncra.Application.Interfaces;

public interface IEventValidatorService
{
    Task<(decimal snapBalance, long snapSequence)?> LoadStartState(string accountId, long serverSequence);
    Task<List<Event>?> FetchEventstoReplay(long lastServerSequence, long newServerSequence, string accountId);
    decimal ReplayEachEventinOrder(decimal current_balance, List<Event> events);
    ApplyEventValidationResult TestApplyNewEvent(Event new_even, decimal test_balance, long snapSequence, int events_replayed);
    Task<ApplyEventValidationResult> AccountChecks(Event new_even, string nodeId);
}