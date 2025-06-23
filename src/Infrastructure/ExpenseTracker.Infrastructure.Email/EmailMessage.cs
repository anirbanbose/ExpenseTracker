using ExpenseTracker.Application.Contracts.Email;

namespace ExpenseTracker.Infrastructure.Email;

public class EmailMessage : IEmailMessage
{
    public List<string> ToList { get; set; } = new();
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }

    public EmailMessage(IEnumerable<string> to, string subject, string body, bool isHtml = false)
    {
        ToList.AddRange(to);
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }
}
