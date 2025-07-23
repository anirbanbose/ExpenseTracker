using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.Application.Contracts.Email;

namespace ExpenseTracker.Infrastructure.Email;

public static class EmailServiceRegistration
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();
        if(emailConfig != null)
            services.AddSingleton<EmailConfiguration>(emailConfig);
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}
