using RecomendationService.Infrastructure;
using RecomendationService.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.InfrastructureServices();
builder.Services.ApplicationServices();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.MapControllers();

app.Run();
