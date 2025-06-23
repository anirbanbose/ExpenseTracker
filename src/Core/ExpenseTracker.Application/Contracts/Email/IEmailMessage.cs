
namespace ExpenseTracker.Application.Contracts.Email;

public interface IEmailMessage
{
    List<string> ToList { get; set; }
    string Subject { get; set; }
    string Body { get; set; }
    bool IsHtml { get; set; }
}
