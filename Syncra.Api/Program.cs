using Syncra.Application;
using Syncra.Infrastructure;
using Syncra.Domain;
using Syncra.Worker;
using Syncra.Worker2;
using Syncra.Worker3;
using Microsoft.OpenApi;
using System.Reflection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Syncra API",
        Description = "A distributed transaction system that supports intermittent connectivity",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddApiServices().AddApplicationServices();
builder.Services.AddInfraServices(builder.Configuration, builder.Configuration.GetConnectionString("localConnectionString")!).AddDomainServices();
builder.Services.AddWorkerServices().AddWorker2Services().AddWorker3Services();

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.Enrich.FromLogContext()
.WriteTo.PostgreSQL(
    connectionString: builder.Configuration.GetConnectionString("localConnectionString")!,
    tableName: "Logs",
    columnOptions: null,
    needAutoCreateTable: false)
.CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Syncra API");
    });
}
app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateAsyncScope())
{
    var dbcontext = scope.ServiceProvider.GetRequiredService<SyncraDbContext>();
    await dbcontext.Database.EnsureCreatedAsync();

    if (!dbcontext.Users.Any())
    {
        var records = DataSeeder.GenerateMockRecords();

        await dbcontext.Users.AddRangeAsync(records["users"].Cast<User>().ToList());
        await dbcontext.NodeStates.AddRangeAsync(records["nodes"].Cast<NodeState>().ToList());
        await dbcontext.Accounts.AddRangeAsync(records["accounts"].Cast<Account>().ToList());
        await dbcontext.AccountStates.AddRangeAsync(records["accountStates"].Cast<AccountState>().ToList());
        await dbcontext.AccountSnapshots.AddRangeAsync(records["snapshots"].Cast<AccountSnapshot>().ToList());
        await dbcontext.Events.AddRangeAsync(records["events"].Cast<Event>().ToList());
        await dbcontext.EventArchives.AddRangeAsync(records["eventArchives"].Cast<EventArchive>().ToList());
        await dbcontext.Conflicts.AddRangeAsync(records["conflicts"].Cast<Conflict>().ToList());
        await dbcontext.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
