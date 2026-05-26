using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncra.Application.DTOs;
using Syncra.Application.Interfaces;
using Syncra.Infrastructure.Repositories;
using worker2 = Syncra.Worker2;
using worker3 = Syncra.Worker3;

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
            config.AddConsumer<worker2.Worker>();
            config.AddConsumer<worker3.Worker>();
            config.UsingRabbitMq((context, config) =>
            {
                config.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]!);
                    h.Password(configuration["RabbitMq:Password"]!);
                });

                config.Message<SyncEventRequest>(x => x.SetEntityName("sync-event-exchange"));
                config.Publish<SyncEventRequest>(x => x.ExchangeType = "x-consistent-hash");

                config.ReceiveEndpoint("queue:transaction-processing-1", options =>
                {
                    options.ConfigureConsumeTopology = false;
                    options.ConcurrentMessageLimit = 20;
                    options.Bind("sync-event-exchange", b =>
                    {
                        b.ExchangeType = "x-consistent-hash";
                        b.RoutingKey = "100";
                    });
                    var partitioner = options.CreatePartitioner(16);
                    options.ConfigureConsumer<Worker.Worker>(context, c =>
                        {
                            c.Message<SyncEventRequest>(m =>
                                {
                                    m.UsePartitioner(partitioner, msg => msg.Message.accountId);
                                });
                        });
                });

                config.ReceiveEndpoint("queue:transaction-processing-2", options =>
                {
                    options.ConfigureConsumeTopology = false;
                    options.ConcurrentMessageLimit = 20;
                    options.Bind("sync-event-exchange", b =>
                    {
                        b.ExchangeType = "x-consistent-hash";
                        b.RoutingKey = "100";
                    });
                    var partitioner = options.CreatePartitioner(16);
                    options.ConfigureConsumer<worker2.Worker>(context, c =>
                        {
                            c.Message<SyncEventRequest>(m =>
                                {
                                    m.UsePartitioner(partitioner, msg => msg.Message.accountId);
                                });
                        });
                });

                config.ReceiveEndpoint("queue:transaction-processing-3", options =>
                {
                    options.ConfigureConsumeTopology = false;
                    options.ConcurrentMessageLimit = 20;
                    options.Bind("sync-event-exchange", b =>
                    {
                        b.ExchangeType = "x-consistent-hash";
                        b.RoutingKey = "100";
                    });
                    var partitioner = options.CreatePartitioner(16);
                    options.ConfigureConsumer<worker3.Worker>(context, c =>
                        {
                            c.Message<SyncEventRequest>(m =>
                                {
                                    m.UsePartitioner(partitioner, msg => msg.Message.accountId);
                                });
                        });
                });
                config.ReceiveEndpoint("queue:transactions-idem", options =>
                {
                    options.ConfigureConsumeTopology = false;
                    options.Bind("sync-event-exchange", b =>
                    {
                        b.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;
                    });
                    options.SetQueueArgument("x-message-ttl", 172800000);
                });
            });
        }
        );
        services.AddScoped<IIdempotencyKeysRepository, IdempotencyKeysRepository>();
        return services;
    }
}