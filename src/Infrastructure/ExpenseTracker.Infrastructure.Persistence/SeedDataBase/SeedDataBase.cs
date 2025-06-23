using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace ExpenseTracker.Infrastructure.Persistence;
public interface ISeedDataBase
{
    Task Seed();
}
public class SeedDataBase : ISeedDataBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IConfiguration _configuration;

    private record MockCurrency(string Code, string? Symbol, string Name);
    private record MockUser(string Email, string Password, string FirstName, string LastName, string PreferredCurrencyCode
        , List<string> UserExpenseCategories, List<MockExpense> Expenses);

    private record MockExpense(decimal Amount, string Category, string Description, string? CurrencyCode = null);    

    private record MockData(List<MockCurrency> Currencies, List<string> ExpenseCategories, MockUser User);

    public SeedDataBase(IUserRepository userRepository, IUnitOfWork unitOfWork
        , ICurrencyRepository currencyRepository, IExpenseCategoryRepository expenseCategoryRepository, IExpenseRepository expenseRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currencyRepository = currencyRepository;
        _expenseCategoryRepository = expenseCategoryRepository;
        _expenseRepository = expenseRepository;
        _configuration = configuration;
    }

    public async Task Seed()
    {
        try
        {            
            MockData? mockData = ReadMockDataFromJsonFile();
            if(mockData is null)
            {
                throw new Exception("Could not read Seed data. Please check the seed data file.");
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100));
            var currencies = await InsertCurrenciesIfMissing(mockData.Currencies, cts.Token);
            var expenseCategories = await InsertExpenseCategoriesIfMissing(mockData.ExpenseCategories, cts.Token);
            await SeedTestUser(mockData.User, currencies, expenseCategories);
            await _unitOfWork.CommitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Data seeding was canceled due to timeout.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
        }
    }

    private MockData? ReadMockDataFromJsonFile()
    {
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "SeedDataBase", "expense_tracker_mock_data.json");
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Seed file not found at path: {jsonPath}");

        var json = File.ReadAllText(jsonPath);
        var mockData = JsonSerializer.Deserialize<MockData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return mockData;
    }


    private async Task<List<Currency>> InsertCurrenciesIfMissing(List<MockCurrency> _mockCurrencies, CancellationToken cancellationToken)
    {
        var currencies = (await _currencyRepository.GetAllCurrenciesAsync(cancellationToken)).ToList();
        if(!currencies.Any())
        {
            foreach (var c in _mockCurrencies)
            {
                var modelResult = Currency.Create(c.Code, c.Name, c.Symbol);
                if (modelResult is not null && modelResult.IsSuccess)
                {
                    currencies.Add(modelResult.Value);
                    await _currencyRepository.AddCurrencyAsync(modelResult.Value);
                }
            }
        }        
        return currencies;
    }

    private async Task<List<ExpenseCategory>> InsertExpenseCategoriesIfMissing(List<string> _mockExpenseCategories, CancellationToken cancellationToken)
    {
        var expenseCategories = (await _expenseCategoryRepository.GetAllSystemExpenseCategoriesAsync(cancellationToken)).ToList();
        if (!expenseCategories.Any())
        {
            foreach (var c in _mockExpenseCategories)
            {
                var model = new ExpenseCategory(c, true);

                expenseCategories.Add(model);
                await _expenseCategoryRepository.AddExpenseCategoryAsync(model);
            }
        }
        return expenseCategories;
    }

    private async Task  SeedTestUser(MockUser mockUser, List<Currency> currencies, List<ExpenseCategory> expenseCategories)
    {
        var seedTestUserData = Convert.ToBoolean(_configuration["SeedTestUserData:Enabled"]);
        if(seedTestUserData)
        {
            var preferredCurrency = currencies.FirstOrDefault(c => c.Code == mockUser.PreferredCurrencyCode);
            if(preferredCurrency is null)
            {
                throw new Exception($"Preferred currency {mockUser.PreferredCurrencyCode} not found.");
            }
            var userResult = User.Create(_userRepository, mockUser.Email, mockUser.Password, preferredCurrency, mockUser.FirstName, mockUser.LastName);
            if (userResult.IsSuccess)
            {
                var user = userResult.Value;
                await _userRepository.AddUserAsync(user);
                foreach (var category in mockUser.UserExpenseCategories)
                {
                    var expenseCategory = new ExpenseCategory(category, false, user.Id);
                    await _expenseCategoryRepository.AddExpenseCategoryAsync(expenseCategory);
                    expenseCategories.Add(expenseCategory);                  
                }
                List<DateTime> mockDates = GetMockDates(mockUser.Expenses.Count);
                int dateIndex = 0;
                
                foreach (var expense in mockUser.Expenses)
                {
                    var category = expenseCategories.FirstOrDefault(ec => ec.Name == expense.Category);
                    Currency? currency = !string.IsNullOrEmpty(expense.CurrencyCode) ? currencies.FirstOrDefault(c => c.Code == expense.CurrencyCode) : null; 
                    if (category is not null)
                    {
                        var expenseModel = new Expense(
                            new Money(
                                expense.Amount, 
                                currency is not null? currency.Id : preferredCurrency.Id, 
                                currency is not null ? currency.Code : preferredCurrency.Code, 
                                currency is not null ? currency.Symbol : preferredCurrency.Symbol
                                ),
                            expense.Description,
                            category.Id,
                            mockDates[dateIndex],
                            user.Id
                        );
                        await _expenseRepository.AddExpenseAsync(expenseModel);
                        dateIndex++;
                    }
                }
            }
        }       
        
    }

    private static List<DateTime> GetMockDates(int count)
    {
        Random random = new Random();
        DateTime maxDate = DateTime.Today;
        DateTime minDate = maxDate.AddYears(-3); // 3 years ago
        List<DateTime> dates = new List<DateTime>();
        for (int i = 0; i < count; i++)
        {
            dates.Add(random.GetRandomDate(minDate, maxDate));
        }
        return dates;
    }
}
