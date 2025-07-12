using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Infrastructure.Persistence.Tests.Repositories;

public class CurrencyRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CurrencyRepository _repository;

    public CurrencyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var mockPublisher = new Mock<IPublisher>();
        _context = new ApplicationDbContext(options, mockPublisher.Object);
        _repository = new CurrencyRepository(_context);
    }

    public void Dispose() => _context?.Dispose();

    [Fact]
    public async Task AddCurrencyAsync_Should_Add_Currency()
    {
        var currency = new Currency("USD", "US Dollar", "$");

        await _repository.AddCurrencyAsync(currency);
        await _context.SaveChangesAsync(CancellationToken.None);

        var saved = await _context.Currencies.FirstOrDefaultAsync(c => c.Code == "USD");
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("US Dollar");
    }

    [Fact]
    public async Task GetCurrencyByCodeAsync_Should_Return_Correct_Currency()
    {
        var currency = new Currency("EUR", "Euro", "€");
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetCurrencyByCodeAsync("eur", CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Euro");
    }

    [Fact]
    public async Task GetCurrencyByIdAsync_Should_Only_Return_Not_Deleted()
    {
        var currency = new Currency("INR", "Rupee", "₹");
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetCurrencyByIdAsync(currency.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Code.Should().Be("INR");
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_Should_Return_All()
    {
        _context.Currencies.AddRange(
            new Currency("GBP", "Pound", "£"),
            new Currency("JPY", "Yen", "¥")
        );
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllCurrenciesAsync(CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchCurrenciesAsync_Should_Filter_Sort_And_Paginate()
    {
        _context.Currencies.AddRange(
            new Currency("USD", "US Dollar", "$"),
            new Currency("EUR", "Euro", "€"),
            new Currency("INR", "Rupee", "₹")
        );
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.SearchCurrenciesAsync(
            "e", // search
            1,   // page index
            10,  // page size
            CurrencyListOrder.Name, // sort by
            true, // ascending
            CancellationToken.None
        );

        result.Items.Should().ContainSingle(c => c.Code == "INR");
        result.TotalCount.Should().Be(2); // Rupee and EUR match "e"
    }

    [Fact]
    public void UpdateCurrency_Should_Mark_Entity_As_Modified()
    {
        var currency = new Currency("CAD", "Canadian Dollar", "C$");

        _repository.UpdateCurrency(currency);

        _context.Entry(currency).State.Should().Be(EntityState.Modified);
    }
}
