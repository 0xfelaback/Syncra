using Syncra.Worker3;

var builder = Host.CreateApplicationBuilder(args);

var host = builder.Build();
host.Run();
