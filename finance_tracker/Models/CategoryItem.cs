using SQLite;

namespace FinanceTracker.Models;

[Table("Categories")]
public class CategoryItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Name { get; set; } = string.Empty;

    public TransactionType Type { get; set; } = TransactionType.Expense;

    public string Icon { get; set; } = "💰";

    public string ColorHex { get; set; } = "#6366F1";

    public decimal MonthlyBudgetLimit { get; set; } = 0;
}
