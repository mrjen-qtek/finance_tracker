namespace FinanceTracker.Models;

public class FinancialSummary
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance => TotalIncome - TotalExpense;
    public double SavingsRate => TotalIncome > 0 ? (double)((TotalIncome - TotalExpense) / TotalIncome) * 100 : 0;
    public int TransactionCount { get; set; }

    public string FormattedTotalIncome => $"${TotalIncome:N2}";
    public string FormattedTotalExpense => $"${TotalExpense:N2}";
    public string FormattedNetBalance => $"${NetBalance:N2}";
    public string FormattedSavingsRate => $"{SavingsRate:F1}%";
}
