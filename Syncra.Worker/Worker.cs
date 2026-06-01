using MassTransit;
using System.Text.Json;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;

namespace Syncra.Worker;

public class Worker : IConsumer<Event>, IWorker
// ensured events idempotency
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Worker> _logger;
    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<Event> context)
    {
        // sync transactions
        var message = context.Message;
        var accountId = message.aggregateId;
        bool idempotency = default;
        if (string.IsNullOrWhiteSpace(accountId))
        {
            string errorMsg = $"An Event holding no accountId in database happened to be transmitted; eventId: {message.event_id}, accountId: {accountId}";
            _logger.LogError(errorMsg);
            throw new NullReferenceException(errorMsg);
        }
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var idempotencyRepo = scope.ServiceProvider.GetRequiredService<IIdempotencyKeysRepository>();
            idempotency = await idempotencyRepo.CheckThatEventIdExists(message.event_id);
        }
        if (idempotency)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(new Uri("queue:transactions-idem"));
            await endpoint.Send(message);
            _logger.LogInformation($"An idempotency action was performed for eventId: {message.event_id}");
            await Task.CompletedTask; // send the event to idempotency queue
            return;
        }

        using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var accountSnapRepo = scope.ServiceProvider.GetRequiredService<IAccountSnapshotRepository>();
            var conflictRepo = scope.ServiceProvider.GetRequiredService<IConflictRepository>();
            var idempotencyRepo = scope.ServiceProvider.GetRequiredService<IIdempotencyKeysRepository>();
            var eventValidatorService = scope.ServiceProvider.GetRequiredService<IEventValidatorService>();
            ApplyEventValidationResult accountCheck = await eventValidatorService.AccountChecks(message, message.node_id);
            if (!accountCheck.isValid)
            {
                _logger.LogWarning("Event {EventId} rejected during account checks: {Reason}", message.event_id, accountCheck.reason);
                await idempotencyRepo.AddAsync(new IdempotencyKey
                {
                    event_id = message.event_id,
                    response_status = 400,
                    response_body = JsonDocument.Parse("{\"status\":\"rejected\",\"reason\":\"account_check_failed\"}") // TODO: not completely implemented yet.
                });
                await idempotencyRepo.SaveChangesAsync();
                await Task.CompletedTask;
                return;
            }
            Event? even = await eventRepo.GetByIdAsync(message.event_id);
            if (even is null)
            {
                string errorMsg2 = $"An Event that is not recorded in database happened to be transmitted: {message.event_id}";
                _logger.LogError(errorMsg2);
                throw new NullReferenceException(errorMsg2);
            }
            long? lastServerSequence = await eventRepo.GetLastServerSequence();
            long serverSequence = lastServerSequence!.Value + 1; // lastServerSequence.GetValueOrDefault(0) + 1;
            (decimal snapBalance, long snapSequence)? result = await eventValidatorService.LoadStartState(accountId, serverSequence);
            decimal pre_new_event_balance;
            long replayFromSequence;
            if (result is null)
            {
                _logger.LogInformation("No snapshot found for account {AccountId}. Starting replay from zero.", accountId);
                pre_new_event_balance = 0m;
                replayFromSequence = 0;
            }
            else
            {
                pre_new_event_balance = result.Value.snapBalance;
                replayFromSequence = result.Value.snapSequence;
            }

            var events = await eventValidatorService.FetchEventstoReplay(replayFromSequence, serverSequence, accountId) ?? new List<Event>();
            int eventReplayed = events.Count;
            decimal currentBalance = eventValidatorService.ReplayEachEventinOrder(pre_new_event_balance, events);
            ApplyEventValidationResult validation = eventValidatorService.TestApplyNewEvent(even, currentBalance, replayFromSequence, eventReplayed);
            if (!validation.isValid)
            {
                // compensate event
                long? lastServerSequenceforCompensate = await eventRepo.GetLastServerSequence();
                even.Status = Event.EventStatus.Compensated;

                long compensationSequence = lastServerSequenceforCompensate!.Value + 1;
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
                    Status = Event.EventStatus.Compensated,
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

                await eventRepo.AddAsync(compensatingEvent);
                await eventRepo.UpdateAsync(even);
                await conflictRepo.AddAsync(conflict);
                await idempotencyRepo.AddAsync(new IdempotencyKey
                {
                    event_id = message.event_id,
                    response_status = 409,
                    response_body = JsonDocument.Parse("{\"status\":\"conflict\",\"reason\":\"event_validation_failed\"}")
                });

                await eventRepo.SaveChangesAsync();
                await conflictRepo.SaveChangesAsync();
                await idempotencyRepo.SaveChangesAsync();

                _logger.LogWarning("Conflict detected and compensated for event {EventId}. Reason: {Reason}", even.event_id, validation.reason);
                await Task.CompletedTask;
                return;
            }

            // accept transaction

            even.Status = Event.EventStatus.Accepted;
            await eventRepo.UpdateAsync(even);

            if (serverSequence % 100 == 0)
            {
                AccountSnapshot snapshot = new AccountSnapshot
                {
                    account_id = accountId,
                    snapshot_sequence = serverSequence,
                    balance = validation.postEventBalance,
                    event_count = serverSequence == 0 ? 0 : 100
                };
                await accountSnapRepo.AddAsync(snapshot);
            }

            await idempotencyRepo.AddAsync(new IdempotencyKey
            {
                event_id = message.event_id,
                response_status = 200,
                response_body = JsonDocument.Parse("{\"status\":\"accepted\"}")
            });

            await eventRepo.SaveChangesAsync();
            await idempotencyRepo.SaveChangesAsync();

            _logger.LogInformation("Accepted event {EventId} with server_sequence {ServerSequence}", message.event_id, serverSequence);

        }





        throw new NotImplementedException();
    }
}

public interface IWorker
{
    Task Consume(ConsumeContext<Event> context);
}
