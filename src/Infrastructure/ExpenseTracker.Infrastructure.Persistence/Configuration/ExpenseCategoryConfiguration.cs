
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configuration;

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("PK_ExpenseCategory");

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ExpenseCategoryId(value)
            );

        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
        builder.Property(d => d.IsSystemCategory).IsRequired();

        builder.HasOne(t => t.CategoryOwner)
                .WithMany(u => u.ExpenseCategories)
                .HasForeignKey(t => t.CategoryOwnerId).IsRequired(false);

        builder.ToTable("ExpenseCategory", "ET");
    }
}

