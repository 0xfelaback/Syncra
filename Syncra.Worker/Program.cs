using Syncra.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddWorkerServices();

var host = builder.Build();


host.Run();
