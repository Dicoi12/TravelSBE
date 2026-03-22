using Microsoft.Extensions.DependencyInjection;
using TravelsBE.Services.Interfaces;
using TravelSBE.Services;

namespace TravelSBE.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
