namespace FinanceTracker.Models;

public class CategorySummary
{
    public string CategoryName { get; set; } = string.Empty;
    public string Icon { get; set; } = "💰";
    public string ColorHex { get; set; } = "#6366F1";
    public decimal TotalAmount { get; set; }
    public double Percentage { get; set; } // 0.0 to 1.0 for progress bar
    public int TransactionCount { get; set; }
    public decimal BudgetLimit { get; set; }

    public string FormattedTotal => $"${TotalAmount:N2}";
    public string FormattedPercentage => $"{Percentage * 100:F1}%";

    public double BudgetProgress => BudgetLimit > 0 ? Math.Min(1.0, (double)(TotalAmount / BudgetLimit)) : 0;
    public string BudgetStatusText => BudgetLimit > 0
        ? (TotalAmount <= BudgetLimit
            ? $"${BudgetLimit - TotalAmount:N2} left of ${BudgetLimit:N0}"
            : $"Over by ${TotalAmount - BudgetLimit:N2} (limit: ${BudgetLimit:N0})")
        : "No budget limit set";

    public string BudgetStatusColor => BudgetLimit <= 0
        ? "#94A3B8"
        : (TotalAmount > BudgetLimit
            ? "#EF4444"
            : (TotalAmount >= BudgetLimit * 0.85m ? "#F59E0B" : "#10B981"));
}
