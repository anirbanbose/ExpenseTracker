using ExpenseTracker.Application.Contracts.BackgroundJob;
using ExpenseTracker.Application.Contracts.Email;

namespace ExpenseTracker.Infrastructure.BackgroundJobs.EmailJob;

public class SendEmailBackgroundJob : IEmailBackgroundJob
{
    private readonly IEmailSender _emailSender;

    public SendEmailBackgroundJob(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendEmailAsync(List<string> emails, string subject, string body)
    {
        await _emailSender.SendEmail(subject, body, emails);
    }
}
