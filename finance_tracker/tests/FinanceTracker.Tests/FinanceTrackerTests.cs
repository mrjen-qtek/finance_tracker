using FinanceTracker.Converters;
using FinanceTracker.Models;
using FinanceTracker.Services;
using System.Globalization;

namespace FinanceTracker.Tests;

public class FinanceTrackerModelTests
{
    [Fact]
    public void TransactionItem_Income_FormatsCorrectly()
    {
        var item = new TransactionItem
        {
            Title = "Freelance Project",
            Amount = 1500.50m,
            Type = TransactionType.Income,
            TransactionDate = new DateTime(2026, 10, 11)
        };

        Assert.True(item.IsIncome);
        Assert.False(item.IsExpense);
        Assert.Equal("+$1,500.50", item.FormattedAmount);
        Assert.Equal("#10B981", item.AmountColorHex);
        Assert.Equal("Oct 11, 2026", item.FormattedDate);
    }

    [Fact]
    public void TransactionItem_Expense_FormatsCorrectly()
    {
        var item = new TransactionItem
        {
            Title = "Dinner",
            Amount = 85.25m,
            Type = TransactionType.Expense,
            TransactionDate = new DateTime(2026, 10, 10)
        };

        Assert.False(item.IsIncome);
        Assert.True(item.IsExpense);
        Assert.Equal("-$85.25", item.FormattedAmount);
        Assert.Equal("#EF4444", item.AmountColorHex);
    }

    [Fact]
    public void FinancialSummary_NetBalanceAndSavingsRate_CalculatesAccurately()
    {
        var summary = new FinancialSummary
        {
            TotalIncome = 5000.00m,
            TotalExpense = 2000.00m,
            TransactionCount = 10
        };

        Assert.Equal(3000.00m, summary.NetBalance);
        Assert.Equal(60.0, summary.SavingsRate);
        Assert.Equal("$5,000.00", summary.FormattedTotalIncome);
        Assert.Equal("$2,000.00", summary.FormattedTotalExpense);
        Assert.Equal("$3,000.00", summary.FormattedNetBalance);
        Assert.Equal("60.0%", summary.FormattedSavingsRate);
    }

    [Fact]
    public void CategorySummary_BudgetTracking_CalculatesStatusAccurately()
    {
        // Safe budget (well below limit)
        var safe = new CategorySummary
        {
            CategoryName = "Groceries",
            TotalAmount = 300.00m,
            BudgetLimit = 500.00m
        };
        Assert.Equal(0.6, safe.BudgetProgress);
        Assert.Equal("#10B981", safe.BudgetStatusColor); // Green

        // Warning budget (>= 85%)
        var warning = new CategorySummary
        {
            CategoryName = "Dining",
            TotalAmount = 450.00m,
            BudgetLimit = 500.00m
        };
        Assert.Equal(0.9, warning.BudgetProgress);
        Assert.Equal("#F59E0B", warning.BudgetStatusColor); // Amber

        // Exceeded budget (> 100%)
        var exceeded = new CategorySummary
        {
            CategoryName = "Shopping",
            TotalAmount = 550.00m,
            BudgetLimit = 500.00m
        };
        Assert.Equal(1.0, exceeded.BudgetProgress);
        Assert.Equal("#EF4444", exceeded.BudgetStatusColor); // Red
        Assert.Contains("Over by", exceeded.BudgetStatusText);
    }

    [Fact]
    public void SeedData_ReturnsRealisticDefaultData()
    {
        var categories = SeedData.GetDefaultCategories();
        var transactions = SeedData.GetDefaultTransactions();

        Assert.NotEmpty(categories);
        Assert.Contains(categories, c => c.Type == TransactionType.Income);
        Assert.Contains(categories, c => c.Type == TransactionType.Expense);

        Assert.NotEmpty(transactions);
        Assert.Contains(transactions, t => t.Type == TransactionType.Income);
        Assert.Contains(transactions, t => t.Type == TransactionType.Expense);
        Assert.True(transactions.Sum(t => t.Amount) > 0);
    }

    [Fact]
    public void CurrencyConverter_ConvertsValuesProperly()
    {
        var converter = new CurrencyConverter();
        var resultDecimal = converter.Convert(1234.56m, typeof(string), null, CultureInfo.InvariantCulture);
        var resultDouble = converter.Convert(99.99, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("$1,234.56", resultDecimal);
        Assert.Equal("$99.99", resultDouble);
    }

    [Fact]
    public void PercentageConverter_FormatsProperly()
    {
        var converter = new PercentageConverter();
        var result = converter.Convert(0.42, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("42%", result);
    }
}
