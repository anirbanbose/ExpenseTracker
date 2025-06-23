
namespace ExpenseTracker.Application.DTO.Dashboard;

public class ExpenseSummaryDTO
{
    public List<string> TotalExpenses { get; set; }
    public List<string> CurrentMonthExpenses { get; set; }
    public List<CategoryExpenseDTO> CurrentMonthTopCategoryExpense { get; set; }
}

public record CategoryExpenseDTO(string CategoryName, string Expense);

