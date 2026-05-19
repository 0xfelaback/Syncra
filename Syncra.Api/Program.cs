using Syncra.Application;
using Syncra.Infrastructure;
using Syncra.Domain;
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
builder.Services.AddInfraServices(builder.Configuration.GetConnectionString("localConnectionString")!).AddDomainServices();

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

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
