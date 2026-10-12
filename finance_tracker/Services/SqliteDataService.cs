using FinanceTracker.Models;
using SQLite;

namespace FinanceTracker.Services;

public class SqliteDataService : IDataService
{
    private SQLiteAsyncConnection? _database;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isInitialized;

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized && _database != null)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized && _database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financetracker.db3");
            _database = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<CategoryItem>();
            await _database.CreateTableAsync<TransactionItem>();

            var categoryCount = await _database.Table<CategoryItem>().CountAsync();
            if (categoryCount == 0)
            {
                var defaultCategories = SeedData.GetDefaultCategories();
                await _database.InsertAllAsync(defaultCategories);

                var defaultTransactions = SeedData.GetDefaultTransactions();
                await _database.InsertAllAsync(defaultTransactions);
            }

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task InitializeAsync()
    {
        await EnsureInitializedAsync();
    }

    public async Task<List<TransactionItem>> GetTransactionsAsync(
        TransactionType? type = null,
        string? category = null,
        DatePeriodFilter period = DatePeriodFilter.All,
        string? searchQuery = null)
    {
        await EnsureInitializedAsync();
        if (_database == null) return new();

        var query = _database.Table<TransactionItem>();

        if (type.HasValue)
        {
            var targetType = type.Value;
            query = query.Where(t => t.Type == targetType);
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "All" && category != "All Categories")
        {
            query = query.Where(t => t.Category == category);
        }

        var (startDate, endDate) = GetDateRange(period);
        if (startDate.HasValue)
        {
            var start = startDate.Value;
            query = query.Where(t => t.TransactionDate >= start);
        }
        if (endDate.HasValue)
        {
            var end = endDate.Value;
            query = query.Where(t => t.TransactionDate <= end);
        }

        var items = await query.OrderByDescending(t => t.TransactionDate).ThenByDescending(t => t.Id).ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.Trim().ToLowerInvariant();
            items = items.Where(t =>
                (t.Title?.ToLowerInvariant().Contains(q) ?? false) ||
                (t.Category?.ToLowerInvariant().Contains(q) ?? false) ||
                (t.Note?.ToLowerInvariant().Contains(q) ?? false) ||
                t.Amount.ToString("F2").Contains(q)).ToList();
        }

        return items;
    }

    public async Task<TransactionItem?> GetTransactionByIdAsync(int id)
    {
        await EnsureInitializedAsync();
        if (_database == null) return null;
        return await _database.Table<TransactionItem>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<int> SaveTransactionAsync(TransactionItem item)
    {
        await EnsureInitializedAsync();
        if (_database == null) return 0;

        if (item.Id != 0)
        {
            return await _database.UpdateAsync(item);
        }
        else
        {
            return await _database.InsertAsync(item);
        }
    }

    public async Task<int> DeleteTransactionAsync(int id)
    {
        await EnsureInitializedAsync();
        if (_database == null) return 0;

        return await _database.DeleteAsync<TransactionItem>(id);
    }

    public async Task<FinancialSummary> GetFinancialSummaryAsync(DatePeriodFilter period = DatePeriodFilter.ThisMonth)
    {
        await EnsureInitializedAsync();
        var transactions = await GetTransactionsAsync(period: period);

        decimal totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        decimal totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return new FinancialSummary
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TransactionCount = transactions.Count
        };
    }

    public async Task<List<CategorySummary>> GetCategoryBreakdownAsync(
        TransactionType type = TransactionType.Expense,
        DatePeriodFilter period = DatePeriodFilter.ThisMonth)
    {
        await EnsureInitializedAsync();
        var transactions = await GetTransactionsAsync(type: type, period: period);
        var categories = await GetCategoriesAsync(type);

        var categoryMap = categories.ToDictionary(c => c.Name, c => c);
        decimal grandTotal = transactions.Sum(t => t.Amount);

        var groups = transactions
            .GroupBy(t => t.Category)
            .Select(g =>
            {
                var catName = g.Key;
                categoryMap.TryGetValue(catName, out var catMeta);
                decimal sum = g.Sum(x => x.Amount);
                double pct = grandTotal > 0 ? (double)(sum / grandTotal) : 0;

                return new CategorySummary
                {
                    CategoryName = catName,
                    Icon = catMeta?.Icon ?? g.FirstOrDefault()?.CategoryIcon ?? "💰",
                    ColorHex = catMeta?.ColorHex ?? g.FirstOrDefault()?.CategoryColor ?? "#6366F1",
                    TotalAmount = sum,
                    Percentage = pct,
                    TransactionCount = g.Count(),
                    BudgetLimit = catMeta?.MonthlyBudgetLimit ?? 0
                };
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return groups;
    }

    public async Task<List<CategoryItem>> GetCategoriesAsync(TransactionType? type = null)
    {
        await EnsureInitializedAsync();
        if (_database == null) return new();

        var query = _database.Table<CategoryItem>();
        if (type.HasValue)
        {
            var targetType = type.Value;
            query = query.Where(c => c.Type == targetType);
        }

        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(CategoryItem category)
    {
        await EnsureInitializedAsync();
        if (_database == null) return 0;

        if (category.Id != 0)
        {
            return await _database.UpdateAsync(category);
        }
        else
        {
            return await _database.InsertAsync(category);
        }
    }

    public async Task<List<MonthlyTrendItem>> GetMonthlyTrendsAsync(int pastMonths = 6)
    {
        await EnsureInitializedAsync();
        if (_database == null) return new();

        var today = DateTime.Today;
        var startMonth = today.AddMonths(-(pastMonths - 1));
        var startDate = new DateTime(startMonth.Year, startMonth.Month, 1);

        var allTransactions = await _database.Table<TransactionItem>()
            .Where(t => t.TransactionDate >= startDate)
            .ToListAsync();

        var result = new List<MonthlyTrendItem>();

        decimal maxVal = 1;

        for (int i = pastMonths - 1; i >= 0; i--)
        {
            var targetMonth = today.AddMonths(-i);
            var mYear = targetMonth.Year;
            var mMonth = targetMonth.Month;

            var monthTrans = allTransactions
                .Where(t => t.TransactionDate.Year == mYear && t.TransactionDate.Month == mMonth)
                .ToList();

            var inc = monthTrans.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var exp = monthTrans.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            if (inc > maxVal) maxVal = inc;
            if (exp > maxVal) maxVal = exp;

            result.Add(new MonthlyTrendItem
            {
                MonthLabel = targetMonth.ToString("MMM"),
                Income = inc,
                Expense = exp
            });
        }

        // Compute relative height percentages (max 100 height units)
        foreach (var item in result)
        {
            item.RelativeIncomeHeight = maxVal > 0 ? (double)(item.Income / maxVal) * 110.0 : 0;
            item.RelativeExpenseHeight = maxVal > 0 ? (double)(item.Expense / maxVal) * 110.0 : 0;
            if (item.RelativeIncomeHeight < 4 && item.Income > 0) item.RelativeIncomeHeight = 4;
            if (item.RelativeExpenseHeight < 4 && item.Expense > 0) item.RelativeExpenseHeight = 4;
        }

        return result;
    }

    public async Task ResetToDemoDataAsync()
    {
        await EnsureInitializedAsync();
        if (_database == null) return;

        await _database.DeleteAllAsync<TransactionItem>();
        await _database.DeleteAllAsync<CategoryItem>();

        await _database.InsertAllAsync(SeedData.GetDefaultCategories());
        await _database.InsertAllAsync(SeedData.GetDefaultTransactions());
    }

    private static (DateTime? Start, DateTime? End) GetDateRange(DatePeriodFilter period)
    {
        var today = DateTime.Today;
        return period switch
        {
            DatePeriodFilter.Today => (today, today.AddDays(1).AddTicks(-1)),
            DatePeriodFilter.ThisWeek => (today.AddDays(-(int)today.DayOfWeek), today.AddDays(7 - (int)today.DayOfWeek).AddTicks(-1)),
            DatePeriodFilter.ThisMonth => (new DateTime(today.Year, today.Month, 1), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)),
            DatePeriodFilter.ThisYear => (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31, 23, 59, 59)),
            _ => (null, null)
        };
    }
}
