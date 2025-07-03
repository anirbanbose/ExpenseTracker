using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    void UpdateUser(User user);
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}
