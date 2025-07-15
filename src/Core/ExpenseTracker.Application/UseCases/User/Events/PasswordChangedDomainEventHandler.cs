using ExpenseTracker.Application.Contracts.BackgroundJob;
using ExpenseTracker.Domain.DomainEvents;
using MediatR;

namespace ExpenseTracker.Application.UseCases.User.Events;

public class PasswordChangedDomainEventHandler : INotificationHandler<PasswordChangedDomainEvent>
{
    private readonly IBackgroundJobService _backgroundJobService;
    public PasswordChangedDomainEventHandler(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public Task Handle(PasswordChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        string subject = "Welcome to Expense Tracker";
        string body = $"Dear {notification.UserFullName},<br/> <p>Your Expense Tracker password has been changed. Please contact us immediately if it was not you who had done this modification.</p> <p>Thanks, <br/>Expense Tracker team.</p>";
        List<string> emails = [notification.UserEmail];
        _backgroundJobService.Enqueue<IEmailBackgroundJob>(job => job.SendEmailAsync(emails, subject, body));
        return Task.CompletedTask;
    }
}