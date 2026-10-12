using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Models;
using FinanceTracker.Services;

namespace FinanceTracker.ViewModels;

public partial class AnalyticsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    public partial DatePeriodFilter SelectedPeriod { get; set; } = DatePeriodFilter.ThisMonth;

    [ObservableProperty]
    public partial string ActiveTab { get; set; } = "Breakdown"; // "Breakdown", "Trends", "Budgets"

    [ObservableProperty]
    public partial decimal TotalIncome { get; set; }

    [ObservableProperty]
    public partial decimal TotalExpense { get; set; }

    [ObservableProperty]
    public partial decimal NetSavings { get; set; }

    [ObservableProperty]
    public partial double SavingsRate { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial bool HasExpenseData { get; set; }

    [ObservableProperty]
    public partial bool HasTrendsData { get; set; }

    [ObservableProperty]
    public partial bool HasBudgetData { get; set; }

    public ObservableCollection<CategorySummary> ExpenseBreakdown { get; } = new();
    public ObservableCollection<CategorySummary> IncomeBreakdown { get; } = new();
    public ObservableCollection<CategorySummary> BudgetSummaries { get; } = new();
    public ObservableCollection<MonthlyTrendItem> MonthlyTrends { get; } = new();

    public AnalyticsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Analytics & Budgets";
    }

    [RelayCommand]
    public async Task LoadAnalyticsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var summary = await _dataService.GetFinancialSummaryAsync(SelectedPeriod);
            TotalIncome = summary.TotalIncome;
            TotalExpense = summary.TotalExpense;
            NetSavings = summary.NetBalance;
            SavingsRate = summary.SavingsRate;

            // Load expense breakdown
            var expenseCats = await _dataService.GetCategoryBreakdownAsync(TransactionType.Expense, SelectedPeriod);
            ExpenseBreakdown.Clear();
            foreach (var item in expenseCats)
            {
                ExpenseBreakdown.Add(item);
            }
            HasExpenseData = ExpenseBreakdown.Count > 0;

            // Load income breakdown
            var incomeCats = await _dataService.GetCategoryBreakdownAsync(TransactionType.Income, SelectedPeriod);
            IncomeBreakdown.Clear();
            foreach (var item in incomeCats)
            {
                IncomeBreakdown.Add(item);
            }

            // Load budget tracking (categories that have budget > 0)
            BudgetSummaries.Clear();
            foreach (var item in expenseCats.Where(c => c.BudgetLimit > 0))
            {
                BudgetSummaries.Add(item);
            }
            HasBudgetData = BudgetSummaries.Count > 0;

            // Load monthly trends
            var trends = await _dataService.GetMonthlyTrendsAsync(6);
            MonthlyTrends.Clear();
            foreach (var trend in trends)
            {
                MonthlyTrends.Add(trend);
            }
            HasTrendsData = MonthlyTrends.Count > 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Could not load analytics: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task SelectPeriodAsync(string periodName)
    {
        if (Enum.TryParse<DatePeriodFilter>(periodName, out var period))
        {
            SelectedPeriod = period;
            await LoadAnalyticsAsync();
        }
    }

    [RelayCommand]
    public void SwitchTab(string tabName)
    {
        ActiveTab = tabName;
    }
}
