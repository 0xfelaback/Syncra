using MassTransit;
using Syncra.Application.DTOs;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;

// TODO: ENSURE HANDLED DISTRIBUTED LOCKING USING OCC!

namespace Syncra.Worker;

public class Worker : IConsumer<SyncEventRequest>, IWorker
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Worker> _logger;
    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<SyncEventRequest> context)
    {
        // sync transactions
        var message = context.Message;
        var accId = message.accountId;
        bool? idempotency = false;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var idempotencyRepo = scope.ServiceProvider.GetRequiredService<IIdempotencyKeysRepository>();
            idempotency = await idempotencyRepo.checkThatEventIdExists(message.event_id);
        }

        if (idempotency.Value)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(new Uri("queue:transactions-idem"));
            await endpoint.Send(message);
            await Task.CompletedTask;
            return;
        }

        Event transactionEvent = new Event
        {
            event_id = message.event_id,
            parent_event_id = message.parentEventId,
            aggregateId = message.accountId, //node_id = message.no
            //idempotency_record = message.id
            node_sequence = message.nodeSequence,
            node_timestamp = message.nodeTimestamp,
            server_sequence = 1,
            Type = message.eventType,
            payload = new Event.EventPayloadData
            {
                amount = message.payload.amount,
                reason = message.payload.reason,
                from_account_id = message.accountId,
                to_account_id = message.payload.to_account_id
            },
            Status = Event.EventStatus.Pending,
        };
        IdempotencyKey newIdempotency = new IdempotencyKey
        {
            event_id = message.event_id,
        };





        throw new NotImplementedException();
    }
}

public interface IWorker
{
    Task Consume(ConsumeContext<SyncEventRequest> context);
}
