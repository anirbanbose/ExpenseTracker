using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.Application.Contracts.Email;
using ExpenseTracker.Infrastructure.Email;

namespace ExpenseTracker.Infrastructure.Email;

public static class EmailServiceRegistration
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

        services.AddSingleton(emailConfig);
        services.AddScoped<IEmailSender, EmailSender>();
        //services.AddScoped<IAccountEmails, AccountEmails>();

        return services;
    }
}
