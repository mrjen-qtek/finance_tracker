using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceTracker.Models;
using FinanceTracker.Services;

namespace FinanceTracker.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    public partial decimal NetBalance { get; set; }

    [ObservableProperty]
    public partial decimal TotalIncome { get; set; }

    [ObservableProperty]
    public partial decimal TotalExpense { get; set; }

    [ObservableProperty]
    public partial double SavingsRate { get; set; }

    [ObservableProperty]
    public partial DatePeriodFilter SelectedPeriod { get; set; } = DatePeriodFilter.ThisMonth;

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial bool HasRecentTransactions { get; set; }

    [ObservableProperty]
    public partial bool HasTopCategories { get; set; }

    public ObservableCollection<TransactionItem> RecentTransactions { get; } = new();
    public ObservableCollection<CategorySummary> TopSpendingCategories { get; } = new();

    public DashboardViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Financial Overview";
    }

    [RelayCommand]
    public async Task LoadDashboardDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var summary = await _dataService.GetFinancialSummaryAsync(SelectedPeriod);
            NetBalance = summary.NetBalance;
            TotalIncome = summary.TotalIncome;
            TotalExpense = summary.TotalExpense;
            SavingsRate = summary.SavingsRate;

            // Load recent transactions (latest 6)
            var transactions = await _dataService.GetTransactionsAsync(period: SelectedPeriod);
            RecentTransactions.Clear();
            foreach (var item in transactions.Take(6))
            {
                RecentTransactions.Add(item);
            }
            HasRecentTransactions = RecentTransactions.Count > 0;

            // Load top spending categories
            var topCats = await _dataService.GetCategoryBreakdownAsync(TransactionType.Expense, SelectedPeriod);
            TopSpendingCategories.Clear();
            foreach (var cat in topCats.Take(4))
            {
                TopSpendingCategories.Add(cat);
            }
            HasTopCategories = TopSpendingCategories.Count > 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to load dashboard data: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task ChangePeriodAsync(string periodName)
    {
        if (Enum.TryParse<DatePeriodFilter>(periodName, out var period))
        {
            SelectedPeriod = period;
            await LoadDashboardDataAsync();
        }
    }

    [RelayCommand]
    public async Task NavigateToAddExpenseAsync()
    {
        await Shell.Current.GoToAsync($"AddEditTransactionPage?type=Expense");
    }

    [RelayCommand]
    public async Task NavigateToAddIncomeAsync()
    {
        await Shell.Current.GoToAsync($"AddEditTransactionPage?type=Income");
    }

    [RelayCommand]
    public void NavigateToTransactions()
    {
        WeakReferenceMessenger.Default.Send<SwitchTabMessage>(new SwitchTabMessage(1));
    }

    [RelayCommand]
    public async Task TransactionTappedAsync(TransactionItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"AddEditTransactionPage?id={item.Id}");
    }

    [RelayCommand]
    public async Task ResetDemoDataAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Reset Demo Data",
            "Are you sure you want to restore the default sample transactions and categories?",
            "Yes, Reset",
            "Cancel");

        if (confirm)
        {
            await _dataService.ResetToDemoDataAsync();
            await LoadDashboardDataAsync();
            await Shell.Current.DisplayAlertAsync("Success", "Demo financial data restored successfully.", "OK");
        }
    }
}
