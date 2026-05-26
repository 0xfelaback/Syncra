using MassTransit;
using Syncra.Application.DTOs;

namespace Syncra.Worker3;

public class Worker(ILogger<Worker> logger) : IConsumer<SyncEventRequest>, IWorker3
{
    public async Task Consume(ConsumeContext<SyncEventRequest> context)
    {
        var message = context.Message;
        var accId = message.accountId;
        throw new NotImplementedException();
    }


}


public interface IWorker3
{
    Task Consume(ConsumeContext<SyncEventRequest> context);
}