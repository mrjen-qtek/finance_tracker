namespace FinanceTracker.Models;

public enum TransactionType
{
    Expense = 0,
    Income = 1
}

public enum PaymentMethod
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    BankTransfer = 3,
    UPI = 4,
    Other = 5
}

public enum DatePeriodFilter
{
    Today = 0,
    ThisWeek = 1,
    ThisMonth = 2,
    ThisYear = 3,
    All = 4
}
