using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using RecomendationService.Application.RepositoryContracts;
using RecomendationService.Domain;
using RecomendationService.Application.HttpClientContracts;

namespace RecomendationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IProfileInfoProvider, ProfileInfoProvider>(options =>
            {
                options.BaseAddress = new Uri(configuration["ExternalURI"]!);
            });

            services.AddScoped<IUserActivityRepository, UserActivityRepository>();

            var mongoClient = new MongoClient(configuration["ConnectionString"]!);

            var database = mongoClient.GetDatabase("RecomendationService");

            var collection = database.GetCollection<UserActivities>("UserActivities");
          
            services.AddSingleton<IMongoCollection<UserActivities>>(collection);

            return services;
        }
    }
}
