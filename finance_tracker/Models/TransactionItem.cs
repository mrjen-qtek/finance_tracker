using SQLite;

namespace FinanceTracker.Models;

[Table("Transactions")]
public class TransactionItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    [Indexed]
    public TransactionType Type { get; set; } = TransactionType.Expense;

    [Indexed]
    public string Category { get; set; } = "General";

    public string CategoryIcon { get; set; } = "💰";

    public string CategoryColor { get; set; } = "#6366F1";

    [Indexed]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Computed presentation properties (Ignored by SQLite)
    [Ignore]
    public bool IsIncome => Type == TransactionType.Income;

    [Ignore]
    public bool IsExpense => Type == TransactionType.Expense;

    [Ignore]
    public string FormattedAmount => IsIncome
        ? $"+${Amount:N2}"
        : $"-${Amount:N2}";

    [Ignore]
    public string FormattedDate => TransactionDate.ToString("MMM dd, yyyy");

    [Ignore]
    public string AmountColorHex => IsIncome ? "#10B981" : "#EF4444";

    [Ignore]
    public string PaymentMethodDisplay => PaymentMethod switch
    {
        PaymentMethod.CreditCard => "Credit Card",
        PaymentMethod.DebitCard => "Debit Card",
        PaymentMethod.BankTransfer => "Bank Transfer",
        PaymentMethod.Cash => "Cash",
        PaymentMethod.UPI => "UPI / Mobile",
        _ => "Other"
    };
}
