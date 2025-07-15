

using ExpenseTracker.Application.Contracts.BackgroundJob;
using ExpenseTracker.Domain.DomainEvents;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Profile.Events;

public class ProfileUpdatedDomainEventHandler : INotificationHandler<ProfileUpdatedDomainEvent>
{
    private readonly IBackgroundJobService _backgroundJobService;
    public ProfileUpdatedDomainEventHandler(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public Task Handle(ProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        string subject = "Expense Tracker profile has been modified";
        string body = $"Dear {notification.NewFullName},<br/> <p>Your Expense Tracker name has been modified from <strong>{notification.OldFullName}</strong> to <strong>{notification.NewFullName}</strong>. Please contact us immediately if it was not you who had done this modification.</p> <p>Thanks, <br/>Expense Tracker team.</p>";
        List<string> emails = [notification.UserEmail];
        _backgroundJobService.Enqueue<IEmailBackgroundJob>(job => job.SendEmailAsync(emails, subject, body));
        return Task.CompletedTask;
    }
}