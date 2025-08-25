using Microsoft.Extensions.DependencyInjection;

namespace RecomendationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
