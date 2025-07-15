using ExpenseTracker.Application.Contracts.BackgroundJob;
using ExpenseTracker.Infrastructure.BackgroundJobs.EmailJob;
using ExpenseTracker.Infrastructure.BackgroundJobs.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.BackgroundJobs;

public static class BackgroundJobServiceRegistration
{
    public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services, IConfiguration configuration)
    {        
        var hangfireStorageType = configuration["Hangfire:Storage"];
        if(hangfireStorageType != null && hangfireStorageType == "SQL_Server")
        {
            services.AddHangfire(config => 
                config.UseSqlServerStorage(configuration.GetConnectionString("HangfireSqlServerConnection")) //Use 'SQL_Server' for production
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
            );
        }
        else
        {
            services.AddHangfire(config =>                
                config.UseMemoryStorage() //Use 'Memory' for dev/demo. Replace with SQL_Server for production
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
            );
        }

        services.AddHangfireServer();
        services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddScoped<IEmailBackgroundJob, SendEmailBackgroundJob>();        

        return services;
    }
}
