namespace Syncra.Application.Interfaces
{
    public interface IIdempotencyKeysRepository
    {
        Task<bool> checkThatEventIdExists(string eventId);
    }
}