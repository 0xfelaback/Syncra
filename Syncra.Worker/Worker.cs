using MassTransit;
using System.Text.Json;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;
using Syncra.Worker.Services;

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
            _logger.LogInformation("Duplicate event {EventId} detected — acknowledging and skipping processing", message.event_id);
            // nothing else; cached idempotency response will be used by the caller.
            return;
        }

        using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var accountSnapRepo = scope.ServiceProvider.GetRequiredService<IAccountSnapshotRepository>();
            var conflictRepo = scope.ServiceProvider.GetRequiredService<IConflictRepository>();
            var idempotencyRepo = scope.ServiceProvider.GetRequiredService<IIdempotencyKeysRepository>();
            var eventValidatorService = scope.ServiceProvider.GetRequiredService<IEventValidatorService>();
            var resolutionService = scope.ServiceProvider.GetRequiredService<ResolutionService>();
            var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
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
            ApplyEventValidationResult validation = eventValidatorService.TestApplyNewEvent(message, currentBalance, replayFromSequence, eventReplayed);
            if (!validation.isValid)
            {
                await resolutionService.ProcessInvalidTransaction(message, accountId, validation, message.event_id);
                await Task.CompletedTask;
                return;
            }

            // accept transaction

            message.server_sequence = serverSequence;
            message.Status = Event.EventStatus.Accepted;
            await eventRepo.UpdateAsync(message);

            // update account projection after accepting the event
            var accountToUpdate = await accountRepo.GetByIdAsync(accountId);
            if (accountToUpdate != null && accountToUpdate.account_state != null)
            {
                accountToUpdate.account_state.balance = validation.postEventBalance;
                accountToUpdate.account_state.version = accountToUpdate.account_state.version + 1;
                accountToUpdate.account_state.last_event_id = message.event_id;
                accountToUpdate.account_state.last_server_sequence = message.server_sequence;
                accountToUpdate.account_state.updated_at = DateTime.UtcNow;
                await accountRepo.UpdateAsync(accountToUpdate);
            }

            if (message.server_sequence % 100 == 0)
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

            await idempotencyRepo.SaveChangesAsync();

            _logger.LogInformation("Accepted event {EventId} with server_sequence {ServerSequence}", message.event_id, serverSequence);

        }





        throw new NotImplementedException();
    }
}


