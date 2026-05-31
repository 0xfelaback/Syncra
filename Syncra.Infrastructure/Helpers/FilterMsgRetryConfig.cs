using MassTransit;

public class FilterEventMsgRetryConfig : IFilter<ConsumeContext<Event>>
{
    public void Probe(ProbeContext context) => context.CreateFilterScope("eventMsgRetryFilter");

    public async Task Send(ConsumeContext<Event> context, IPipe<ConsumeContext<Event>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception)
        {
            int attempt = context.GetRetryAttempt();
            if (attempt >= 20) throw;

            int sequence = context.Message.node_sequence;
            int baseSpan = sequence < 50 ? sequence * 10 : sequence;

            var random = new Random();
            double jitter = random.NextDouble() * 200;
            var delay = TimeSpan.FromMilliseconds(baseSpan * Math.Pow(1.5, attempt) + jitter);

            await context.Defer(delay);
        }
    }
}