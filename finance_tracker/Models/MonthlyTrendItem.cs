namespace FinanceTracker.Models;

public class MonthlyTrendItem
{
    public string MonthLabel { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }

    public double RelativeIncomeHeight { get; set; } // scaled for bar height
    public double RelativeExpenseHeight { get; set; }

    public string FormattedIncome => $"${Income:N0}";
    public string FormattedExpense => $"${Expense:N0}";
}
