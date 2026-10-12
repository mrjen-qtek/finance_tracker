using FinanceTracker.Models;

namespace FinanceTracker.Services;

public interface IDataService
{
    Task InitializeAsync();
    
    Task<List<TransactionItem>> GetTransactionsAsync(
        TransactionType? type = null,
        string? category = null,
        DatePeriodFilter period = DatePeriodFilter.All,
        string? searchQuery = null);

    Task<TransactionItem?> GetTransactionByIdAsync(int id);
    Task<int> SaveTransactionAsync(TransactionItem item);
    Task<int> DeleteTransactionAsync(int id);

    Task<FinancialSummary> GetFinancialSummaryAsync(DatePeriodFilter period = DatePeriodFilter.ThisMonth);
    Task<List<CategorySummary>> GetCategoryBreakdownAsync(TransactionType type = TransactionType.Expense, DatePeriodFilter period = DatePeriodFilter.ThisMonth);
    Task<List<CategoryItem>> GetCategoriesAsync(TransactionType? type = null);
    Task<int> SaveCategoryAsync(CategoryItem category);
    Task<List<MonthlyTrendItem>> GetMonthlyTrendsAsync(int pastMonths = 6);
    Task ResetToDemoDataAsync();
}
