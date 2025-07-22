
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ExpenseTracker.Infrastructure.Persistence.Configuration;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("PK_Currency");

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new CurrencyId(value)
            );


        builder.Property(x => x.Code).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Symbol).HasMaxLength(5).IsRequired(false);

        builder.ToTable("Currency", "ET");
    }
}
