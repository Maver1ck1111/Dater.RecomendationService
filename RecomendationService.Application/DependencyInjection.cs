using Microsoft.Extensions.DependencyInjection;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Application.Services;

namespace RecomendationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRecommendationService, RecommendationService>();
            return services;
        }
    }
}
