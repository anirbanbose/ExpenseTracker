
namespace ExpenseTracker.Domain.Persistence.SearchModels;

public record ExpenseCategorySearchModel : BaseSearchModel
{
    private ExpenseCategorySearchModel(string? searchText) : base(searchText)
    {
    }

    public static ExpenseCategorySearchModel Create(string? searchText = null)
    {
        return new ExpenseCategorySearchModel(searchText);
    }
};
