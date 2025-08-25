using Microsoft.Extensions.DependencyInjection;

namespace RecomendationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
