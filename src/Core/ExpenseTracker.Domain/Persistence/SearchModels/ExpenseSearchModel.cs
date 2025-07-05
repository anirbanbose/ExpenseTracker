
namespace ExpenseTracker.Domain.Persistence.SearchModels;

public record ExpenseSearchModel : BaseSearchModel
{
    public Guid? ExpenseCategoryId { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    private ExpenseSearchModel(string? searchText, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate) : base(searchText)
    {
        ExpenseCategoryId = expenseCategoryId;
        StartDate = startDate;
        EndDate = endDate;
    }
    public static ExpenseSearchModel Create(string? searchText = null, Guid? expenseCategoryId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        return new ExpenseSearchModel(searchText, expenseCategoryId, startDate, endDate);
    }
}
