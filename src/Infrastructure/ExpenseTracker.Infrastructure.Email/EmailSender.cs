using MimeKit;
using ExpenseTracker.Application.Contracts.Email;
using MailKit.Net.Smtp;

namespace ExpenseTracker.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _configuration;

    public EmailSender(EmailConfiguration configuration)
    {
        _configuration = configuration;
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
        email.From.Add(new MailboxAddress("Survey Manager", _configuration.From));
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

            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

        }
    }
}
