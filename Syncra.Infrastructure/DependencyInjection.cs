using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncra.Application.Interfaces;
using Syncra.Infrastructure.Repositories;

namespace Syncra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration, string connString)
    {
        services.AddDbContext<SyncraDbContext>(options => options.UseNpgsql(connString));
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumer<Worker.Worker>();
            config.UsingRabbitMq((context, config) =>
            {
                config.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]!);
                    h.Password(configuration["RabbitMq:Password"]!);
                });

                config.Message<Event>(x => x.SetEntityName("sync-event-exchange"));
                config.Publish<Event>(x => x.ExchangeType = "x-consistent-hash");

                config.ReceiveEndpoint("transaction-processing", options =>
                {
                    options.ConfigureConsumeTopology = false;
                    options.ConcurrentMessageLimit = 20;
                    options.Bind("sync-event-exchange", b =>
                    {
                        b.ExchangeType = "x-consistent-hash";
                        b.RoutingKey = "100";
                    });
                    var partitioner = options.CreatePartitioner(20);
                    options.ConfigureConsumer<Worker.Worker>(context, c =>
                        {
                            c.Message<Event>(m =>
                                {
                                    m.UsePartitioner(partitioner, msg => msg.Message.aggregateId);  // 20 unique account Ids handled at once - similar Ids in sequential order.
                                    m.UseFilter(new FilterEventMsgRetryConfig());
                                });
                        });


                });


            });
        }
        );
        services.AddScoped<IIdempotencyKeysRepository, IdempotencyKeysRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IConflictRepository, ConflictRepository>();
        services.AddScoped<IAccountSnapshotRepository, AccountSnapshotRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<INodeStateRepository, NodeStateRepository>();
        return services;
    }
}