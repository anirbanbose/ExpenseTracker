using MediatR;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class EmailAvailableQuery : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;
    public EmailAvailableQuery(string email)
    {
        Email = email;
    }
}
