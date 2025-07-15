using MimeKit;
using ExpenseTracker.Application.Contracts.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(EmailConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmail(IEmailMessage email)
    {
        var message = CreateEmailMessage(email);
        await SendAsync(message);
    }


    public async Task SendEmail(string subject, string body, List<string> to)
    {
        EmailMessage email = new EmailMessage(to, subject, body, true);
        await SendEmail(email);
    }

    private MimeMessage CreateEmailMessage(IEmailMessage message)
    {
        MimeMessage email = new MimeMessage();
        email.From.Add(new MailboxAddress("Expense Tracker", _configuration.From));
        email.Subject = message.Subject;

        if (message.IsHtml)
        {
            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = message.Body;
            email.Body = builder.ToMessageBody();
        }
        else
        {
            email.Body = new TextPart("plain") { Text = message.Body };
        }

        email.To.AddRange(message.ToList.Select(d => new MailboxAddress("To", d)));
        return email;

    }

    private async Task SendAsync(MimeMessage message)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_configuration.UserName, _configuration.Password);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending an email.");
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

        }
    }
}
