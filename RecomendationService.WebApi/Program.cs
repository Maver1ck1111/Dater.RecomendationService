using RecomendationService.Infrastructure;
using RecomendationService.Application;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.InfrastructureServices();
builder.Services.ApplicationServices();

var app = builder.Build();

app.MapControllers();

app.Run();
