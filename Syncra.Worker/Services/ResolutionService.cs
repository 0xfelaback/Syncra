using System.Text.Json;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;

namespace Syncra.Worker.Services;

public class ResolutionService : IResolutionService
{
    private readonly IEventRepository _eventRepository;
    private readonly IConflictRepository _conflictRepository;
    private readonly IIdempotencyKeysRepository _idempotencyKeysRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<ResolutionService> _logger;

    public ResolutionService(IEventRepository eventRepository, IConflictRepository conflictRepository, IIdempotencyKeysRepository idempotencyKeysRepository, IAccountRepository accountRepository, ILogger<ResolutionService> logger)
    {
        _eventRepository = eventRepository;
        _conflictRepository = conflictRepository;
        _idempotencyKeysRepository = idempotencyKeysRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task ProcessInvalidTransaction(Event even, string accountId, ApplyEventValidationResult validation, string messageEventId)
    {
        long? lastServerSequence = await _eventRepository.GetLastServerSequence();
        long compensationSequence = lastServerSequence!.Value + 1;

        Event compensatingEvent = new Event
        {
            event_id = $"comp_{Guid.NewGuid()}",
            parent_event_id = null,
            aggregateId = accountId,
            compensates_event_id = even.event_id,
            node_id = even.node_id,
            node_sequence = even.node_sequence,
            server_sequence = compensationSequence,
            node_timestamp = even.node_timestamp,
            server_timestamp = DateTime.UtcNow,
            Type = Event.EventType.CompensatingTransaction,
            payload = new Event.EventPayloadData
            {
                amount = even.payload.amount,
                reason = $"Compensation for failed event: {validation.reason}",
                from_account_id = even.payload.from_account_id,
                to_account_id = even.payload.to_account_id
            },
            Status = Event.EventStatus.Accepted,
            created_at = DateTime.UtcNow
        };

        Conflict conflict = new Conflict
        {
            account_id = accountId,
            original_event_id = even.event_id,
            compensation_event_id = compensatingEvent.event_id,
            Type = validation.reason.Contains("InsufficientFunds") ? Conflict.ConflictType.InsufficientFunds : Conflict.ConflictType.InvalidOperation,
            atempted_balance = validation.postEventBalance,
            actual_balance = validation.preEventBalance,
            resolution = Conflict.Resolution.compensate,
            detected_at = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(compensatingEvent);
        await _conflictRepository.AddAsync(conflict);
        await _idempotencyKeysRepository.AddAsync(new IdempotencyKey
        {
            event_id = messageEventId,
            response_status = 409,
            response_body = JsonDocument.Parse("{\"status\":\"conflict\",\"reason\":\"event_validation_failed\"}")
        });

        // update projection: revert account state to pre-event balance
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account != null && account.account_state != null)
        {
            account.account_state.balance = validation.preEventBalance;
            account.account_state.version = account.account_state.version + 1;
            account.account_state.last_event_id = compensatingEvent.event_id;
            account.account_state.last_server_sequence = compensatingEvent.server_sequence;
            account.account_state.updated_at = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);
        }

        _logger.LogWarning("Conflict detected for event {EventId}. Created compensation {CompEventId}. Reason: {Reason}", even.event_id, compensatingEvent.event_id, validation.reason);
    }
}