
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("PK_User");

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value)
            );

        builder.Property(d => d.Email).HasMaxLength(250).IsRequired();
        builder.Property(d => d.PasswordHash).IsRequired();
        builder.Property(d => d.PasswordSalt).IsRequired();

        builder.HasOne(u => u.Preference)
               .WithOne(p => p.User)
               .HasForeignKey<UserPreference>(p => p.Id)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder
            .OwnsOne(e => e.Name, name =>
            {
                name.Property(a => a.FirstName).HasMaxLength(100).IsRequired().HasColumnName("FirstName");
                name.Property(a => a.LastName).HasMaxLength(100).IsRequired().HasColumnName("LastName");
                name.Property(a => a.MiddleName).HasMaxLength(100).IsRequired(false).HasColumnName("MiddleName");
            });

        builder.ToTable("User");
    }
}
