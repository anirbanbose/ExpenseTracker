
using ExpenseTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configuration;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("PK_Expense");

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ExpenseId(value)
            );


        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.ExpenseDate).IsRequired();

        builder.HasOne(t => t.ExpenseOwner)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.ExpenseOwnerId).IsRequired();

        builder.HasOne(t => t.Category)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CategoryId).IsRequired();        

        builder.OwnsOne(e => e.ExpenseAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasColumnName("Amount");

            money.Property(m => m.CurrencyId)
                .HasColumnType("uniqueidentifier")
                .IsRequired()
                .HasColumnName("CurrencyId");

            money.Property(m => m.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired()
                .HasColumnName("CurrencyCode");

            money.Property(m => m.CurrencySymbol)
                .HasMaxLength(5)
                .HasColumnName("CurrencySymbol");
        });


        builder.ToTable("Expense");
    }
}
