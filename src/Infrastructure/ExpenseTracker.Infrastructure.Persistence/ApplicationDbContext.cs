using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ExpenseTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IPublisher _publisher;
    public ApplicationDbContext(DbContextOptions options, IPublisher publisher)
    : base(options)
    {
        _publisher = publisher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        ConfigureEntityDates();

        var entities = ChangeTracker.Entries<IEntity>()
                            .Select(x => x.Entity)
                            .Where(x => x.DomainEvents.Any()).ToList();

        var domainEvents = entities.SelectMany(d => d.DomainEvents).ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories{ get; set; }
    public DbSet<Expense> Expenses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ConfigureEntityDates()
    {
        var addedEntities = ChangeTracker.Entries<IEntity>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);

        var updatedEntities = ChangeTracker.Entries<IEntity>().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);


        foreach (var entity in updatedEntities)
        {
            if (entity != null)
            {
                entity.SetTimestampForUpdate();
            }
        }

        foreach (var entity in addedEntities)
        {
            if (entity != null)
            {
                entity.SetTimestampForCreate();
            }
        }
    }
}
