using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(ApplicationServicesRegistration).Assembly;           

            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
