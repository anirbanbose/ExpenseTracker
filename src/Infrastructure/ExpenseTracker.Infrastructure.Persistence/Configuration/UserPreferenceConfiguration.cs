
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configuration;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.HasKey(p => p.Id).HasName("PK_UserPreference"); ;

        builder.Property(p => p.Id)
               .HasConversion(
                   id => id.Value,
                   value => new UserId(value));

        builder.Property(p => p.PreferredCurrencyId)
            .HasConversion(
                id => id.Value,
                value => new CurrencyId(value)
            );

        builder.ToTable("UserPreference");
    }
}
