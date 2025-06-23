
namespace ExpenseTracker.Application.Contracts.Email;

public interface IEmailSender
{
    Task SendEmail(IEmailMessage email);
    Task SendEmail(string subject, string body, List<string> to);
}
