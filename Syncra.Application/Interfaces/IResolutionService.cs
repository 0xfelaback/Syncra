public interface IResolutionService
{
    Task ProcessInvalidTransaction(Event even, string accountId, ApplyEventValidationResult validation, string messageEventId);
}
