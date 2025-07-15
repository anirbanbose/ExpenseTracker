

namespace ExpenseTracker.Application.Contracts.BackgroundJob;

public interface IEmailBackgroundJob : IBackgroundJob
{
    Task SendEmailAsync(List<string> emails, string subject, string body);
}
