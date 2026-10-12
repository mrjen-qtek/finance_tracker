using FinanceTracker.Models;

namespace FinanceTracker.Services;

public static class SeedData
{
    public static List<CategoryItem> GetDefaultCategories()
    {
        return new List<CategoryItem>
        {
            // Expense Categories
            new() { Name = "Food & Dining", Type = TransactionType.Expense, Icon = "🍔", ColorHex = "#F59E0B", MonthlyBudgetLimit = 650 },
            new() { Name = "Groceries", Type = TransactionType.Expense, Icon = "🛒", ColorHex = "#10B981", MonthlyBudgetLimit = 500 },
            new() { Name = "Housing & Rent", Type = TransactionType.Expense, Icon = "🏠", ColorHex = "#6366F1", MonthlyBudgetLimit = 1600 },
            new() { Name = "Transportation", Type = TransactionType.Expense, Icon = "🚗", ColorHex = "#3B82F6", MonthlyBudgetLimit = 250 },
            new() { Name = "Utilities & Bills", Type = TransactionType.Expense, Icon = "⚡", ColorHex = "#8B5CF6", MonthlyBudgetLimit = 300 },
            new() { Name = "Entertainment", Type = TransactionType.Expense, Icon = "🎬", ColorHex = "#EC4899", MonthlyBudgetLimit = 200 },
            new() { Name = "Shopping", Type = TransactionType.Expense, Icon = "🛍️", ColorHex = "#F43F5E", MonthlyBudgetLimit = 400 },
            new() { Name = "Health & Fitness", Type = TransactionType.Expense, Icon = "🏥", ColorHex = "#14B8A6", MonthlyBudgetLimit = 150 },
            new() { Name = "Subscriptions", Type = TransactionType.Expense, Icon = "📱", ColorHex = "#06B6D4", MonthlyBudgetLimit = 80 },

            // Income Categories
            new() { Name = "Salary", Type = TransactionType.Income, Icon = "💼", ColorHex = "#10B981", MonthlyBudgetLimit = 0 },
            new() { Name = "Freelance & Consulting", Type = TransactionType.Income, Icon = "💻", ColorHex = "#059669", MonthlyBudgetLimit = 0 },
            new() { Name = "Investments & Dividends", Type = TransactionType.Income, Icon = "📈", ColorHex = "#34D399", MonthlyBudgetLimit = 0 },
            new() { Name = "Bonus & Gifts", Type = TransactionType.Income, Icon = "🎁", ColorHex = "#6EE7B7", MonthlyBudgetLimit = 0 }
        };
    }

    public static List<TransactionItem> GetDefaultTransactions()
    {
        var now = DateTime.Today;

        return new List<TransactionItem>
        {
            // Current Month Income
            new()
            {
                Title = "Monthly Tech Salary",
                Amount = 5850.00m,
                Type = TransactionType.Income,
                Category = "Salary",
                CategoryIcon = "💼",
                CategoryColor = "#10B981",
                TransactionDate = new DateTime(now.Year, now.Month, 1),
                PaymentMethod = PaymentMethod.BankTransfer,
                Note = "Direct deposit from corporate payroll"
            },
            new()
            {
                Title = "Freelance Mobile App UI Design",
                Amount = 1200.00m,
                Type = TransactionType.Income,
                Category = "Freelance & Consulting",
                CategoryIcon = "💻",
                CategoryColor = "#059669",
                TransactionDate = now.AddDays(-3),
                PaymentMethod = PaymentMethod.BankTransfer,
                Note = "Milestone payment for client app redesign"
            },
            new()
            {
                Title = "Quarterly Stock Dividends",
                Amount = 345.50m,
                Type = TransactionType.Income,
                Category = "Investments & Dividends",
                CategoryIcon = "📈",
                CategoryColor = "#34D399",
                TransactionDate = now.AddDays(-7),
                PaymentMethod = PaymentMethod.BankTransfer,
                Note = "Index fund dividend distribution"
            },

            // Current Month Expenses
            new()
            {
                Title = "Apartment Rent",
                Amount = 1450.00m,
                Type = TransactionType.Expense,
                Category = "Housing & Rent",
                CategoryIcon = "🏠",
                CategoryColor = "#6366F1",
                TransactionDate = new DateTime(now.Year, now.Month, 2),
                PaymentMethod = PaymentMethod.BankTransfer,
                Note = "October monthly apartment rent"
            },
            new()
            {
                Title = "Supermarket Weekly Groceries",
                Amount = 184.25m,
                Type = TransactionType.Expense,
                Category = "Groceries",
                CategoryIcon = "🛒",
                CategoryColor = "#10B981",
                TransactionDate = now.AddDays(-1),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Whole Foods grocery run"
            },
            new()
            {
                Title = "Organic Market Restock",
                Amount = 96.50m,
                Type = TransactionType.Expense,
                Category = "Groceries",
                CategoryIcon = "🛒",
                CategoryColor = "#10B981",
                TransactionDate = now.AddDays(-5),
                PaymentMethod = PaymentMethod.DebitCard,
                Note = "Fresh vegetables, fruits, and bread"
            },
            new()
            {
                Title = "Bistro Dinner with Friends",
                Amount = 112.80m,
                Type = TransactionType.Expense,
                Category = "Food & Dining",
                CategoryIcon = "🍔",
                CategoryColor = "#F59E0B",
                TransactionDate = now.AddDays(-2),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Dinner and drinks at Italian Trattoria"
            },
            new()
            {
                Title = "Artisan Espresso & Bakery",
                Amount = 18.50m,
                Type = TransactionType.Expense,
                Category = "Food & Dining",
                CategoryIcon = "🍔",
                CategoryColor = "#F59E0B",
                TransactionDate = now,
                PaymentMethod = PaymentMethod.UPI,
                Note = "Morning cold brew and croissant"
            },
            new()
            {
                Title = "Electric & Power Utility",
                Amount = 125.40m,
                Type = TransactionType.Expense,
                Category = "Utilities & Bills",
                CategoryIcon = "⚡",
                CategoryColor = "#8B5CF6",
                TransactionDate = now.AddDays(-4),
                PaymentMethod = PaymentMethod.BankTransfer,
                Note = "Monthly power company bill"
            },
            new()
            {
                Title = "High-Speed Fiber Internet",
                Amount = 79.99m,
                Type = TransactionType.Expense,
                Category = "Utilities & Bills",
                CategoryIcon = "⚡",
                CategoryColor = "#8B5CF6",
                TransactionDate = now.AddDays(-8),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Gigabit home internet"
            },
            new()
            {
                Title = "Rideshare to Airport & Downtown",
                Amount = 48.75m,
                Type = TransactionType.Expense,
                Category = "Transportation",
                CategoryIcon = "🚗",
                CategoryColor = "#3B82F6",
                TransactionDate = now.AddDays(-6),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Uber trip"
            },
            new()
            {
                Title = "Fuel / Gas Station",
                Amount = 62.00m,
                Type = TransactionType.Expense,
                Category = "Transportation",
                CategoryIcon = "🚗",
                CategoryColor = "#3B82F6",
                TransactionDate = now.AddDays(-9),
                PaymentMethod = PaymentMethod.DebitCard,
                Note = "Full tank refill"
            },
            new()
            {
                Title = "Netflix & Spotify Premium",
                Amount = 31.98m,
                Type = TransactionType.Expense,
                Category = "Subscriptions",
                CategoryIcon = "📱",
                CategoryColor = "#06B6D4",
                TransactionDate = now.AddDays(-10),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Monthly streaming subscriptions"
            },
            new()
            {
                Title = "Cinema Tickets & Popcorn",
                Amount = 44.00m,
                Type = TransactionType.Expense,
                Category = "Entertainment",
                CategoryIcon = "🎬",
                CategoryColor = "#EC4899",
                TransactionDate = now.AddDays(-4),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Weekend IMAX movie"
            },
            new()
            {
                Title = "Running Shoes & Fitness Gear",
                Amount = 145.00m,
                Type = TransactionType.Expense,
                Category = "Shopping",
                CategoryIcon = "🛍️",
                CategoryColor = "#F43F5E",
                TransactionDate = now.AddDays(-8),
                PaymentMethod = PaymentMethod.CreditCard,
                Note = "Nike Pegasus road running shoes"
            },
            new()
            {
                Title = "Gym Membership Monthly",
                Amount = 55.00m,
                Type = TransactionType.Expense,
                Category = "Health & Fitness",
                CategoryIcon = "🏥",
                CategoryColor = "#14B8A6",
                TransactionDate = new DateTime(now.Year, now.Month, 3),
                PaymentMethod = PaymentMethod.DebitCard,
                Note = "Climbing & fitness gym membership"
            }
        };
    }
}
