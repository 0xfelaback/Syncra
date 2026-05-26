using MassTransit;
using Syncra.Application.DTOs;

namespace Syncra.Worker2;

public class Worker(ILogger<Worker> logger) : IConsumer<SyncEventRequest>, IWorker2
{
    public async Task Consume(ConsumeContext<SyncEventRequest> context)
    {
        var message = context.Message;
        var accId = message.accountId;
        throw new NotImplementedException();
    }


}


public interface IWorker2
{
    Task Consume(ConsumeContext<SyncEventRequest> context);
}