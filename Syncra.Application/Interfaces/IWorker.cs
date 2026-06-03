using MassTransit;

public interface IWorker
{
    Task Consume(ConsumeContext<Event> context);
}