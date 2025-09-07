using RecomendationService.Infrastructure;
using RecomendationService.Application;
using Serilog;
using RecomendationService.Application.Services;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Application.RepositoryContracts;
using System.Text.Json.Serialization;
using RecomendationService.WebApi.Hubs;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ApplicationServices();

builder.Services.AddSingleton<ConcurrentDictionary<Guid, ConcurrentBag<string>>>();

builder.Services.AddSignalR();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.MapControllers();

app.MapHub<NotificationHub>("/notifications");

app.Run();
