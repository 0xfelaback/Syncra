using MassTransit;
using Syncra.Application.Interfaces;

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

            Event? even = await eventRepo.GetByIdAsync(message.event_id);
            if (even == null)
            {
                _logger.LogError($"An Event that is not recorded in database happened to be transmitted: {message.event_id}");
                throw new NullReferenceException("An Event that is not recorded in database happened to be transmitted");
            }

            long? lastServerSequence = await eventRepo.GetLastServerSequence();
            even.server_sequence = lastServerSequence!.Value + 1;
            _logger.LogInformation($"Assigned last_server_sequence of {lastServerSequence!.Value + 1} to event: {message.event_id}");

        }
        /*IdempotencyKey newIdempotency = new IdempotencyKey
        {
            event_id = message.event_id,
        };*/





        throw new NotImplementedException();
    }
}

public interface IWorker
{
    Task Consume(ConsumeContext<Event> context);
}
