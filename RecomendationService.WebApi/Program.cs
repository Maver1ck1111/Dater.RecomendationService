using RecomendationService.Infrastructure;
using RecomendationService.Application;
using Serilog;
using RecomendationService.Application.Services;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Application.RepositoryContracts;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ApplicationServices();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.MapControllers();

app.Run();
