using ExpenseTracker.Application.Contracts.BackgroundJob;
using ExpenseTracker.Domain.DomainEvents;
using MediatR;

namespace ExpenseTracker.Application.UseCases.User.Events;

public class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IBackgroundJobService _backgroundJobService;
    public UserRegisteredDomainEventHandler(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        string subject = "Welcome to Expense Tracker";
        string body = $"Dear {notification.UserFullName},<br/> <p>Thank you for registering with us.</p> <p>Thanks, <br/>Expense Tracker team.</p>";
        List<string> emails = [notification.UserEmail];
        _backgroundJobService.Enqueue<IEmailBackgroundJob>(job => job.SendEmailAsync(emails, subject, body));
        return Task.CompletedTask;
    }
}
