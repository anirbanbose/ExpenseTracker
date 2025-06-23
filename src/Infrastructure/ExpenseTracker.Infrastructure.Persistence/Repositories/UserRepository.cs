
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Infrastructure.Persistence.Repositories.Common;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
{
    public async Task AddUserAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        await base.AddAsync(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken, bool trackEntity = false)
    {
        IQueryable<User> table = trackEntity ? Table : TableNoTracking;
        return await table
            .Include(d => d.Preference).AsSplitQuery()
            .FirstOrDefaultAsync(d => string.Equals(d.Email.ToLower(), email.ToLower()), cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }


    public void UpdateUser(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        MarkAsUpdated(user);
    }
}
