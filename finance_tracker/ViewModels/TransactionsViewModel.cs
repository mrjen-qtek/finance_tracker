using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Models;
using FinanceTracker.Services;

namespace FinanceTracker.ViewModels;

public partial class TransactionsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    public partial string SelectedTypeFilter { get; set; } = "All"; // "All", "Expenses", "Income"

    [ObservableProperty]
    public partial string SelectedCategoryFilter { get; set; } = "All Categories";

    [ObservableProperty]
    public partial DatePeriodFilter SelectedPeriodFilter { get; set; } = DatePeriodFilter.ThisMonth;

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal FilteredTotal { get; set; }

    [ObservableProperty]
    public partial int FilteredCount { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial bool HasTransactions { get; set; }

    public ObservableCollection<TransactionItem> Transactions { get; } = new();
    public ObservableCollection<string> AvailableCategories { get; } = new();

    public TransactionsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Transactions";
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = LoadTransactionsAsync();
    }

    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            TransactionType? type = SelectedTypeFilter switch
            {
                "Income" => TransactionType.Income,
                "Expenses" => TransactionType.Expense,
                _ => null
            };

            string? cat = SelectedCategoryFilter == "All Categories" ? null : SelectedCategoryFilter;

            var items = await _dataService.GetTransactionsAsync(
                type: type,
                category: cat,
                period: SelectedPeriodFilter,
                searchQuery: SearchText);

            Transactions.Clear();
            foreach (var item in items)
            {
                Transactions.Add(item);
            }

            FilteredCount = items.Count;
            HasTransactions = items.Count > 0;

            if (type == TransactionType.Income)
            {
                FilteredTotal = items.Sum(t => t.Amount);
            }
            else if (type == TransactionType.Expense)
            {
                FilteredTotal = items.Sum(t => t.Amount);
            }
            else
            {
                // Net for current selection
                decimal inc = items.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                decimal exp = items.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                FilteredTotal = inc - exp;
            }

            // Populate category filter options if not yet loaded
            if (AvailableCategories.Count <= 1)
            {
                var categories = await _dataService.GetCategoriesAsync();
                AvailableCategories.Clear();
                AvailableCategories.Add("All Categories");
                foreach (var c in categories.Select(c => c.Name).Distinct())
                {
                    AvailableCategories.Add(c);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Could not load transactions: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task ChangeTypeFilterAsync(string type)
    {
        SelectedTypeFilter = type;
        await LoadTransactionsAsync();
    }

    [RelayCommand]
    public async Task ChangePeriodFilterAsync(string periodStr)
    {
        if (Enum.TryParse<DatePeriodFilter>(periodStr, out var period))
        {
            SelectedPeriodFilter = period;
            await LoadTransactionsAsync();
        }
    }

    [RelayCommand]
    public async Task ChangeCategoryFilterAsync(string category)
    {
        SelectedCategoryFilter = category;
        await LoadTransactionsAsync();
    }

    [RelayCommand]
    public async Task EditTransactionAsync(TransactionItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"AddEditTransactionPage?id={item.Id}");
    }

    [RelayCommand]
    public async Task DeleteTransactionAsync(TransactionItem? item)
    {
        if (item == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Transaction",
            $"Are you sure you want to delete \"{item.Title}\" ({item.FormattedAmount})?",
            "Delete",
            "Cancel");

        if (confirm)
        {
            await _dataService.DeleteTransactionAsync(item.Id);
            Transactions.Remove(item);
            FilteredCount = Transactions.Count;
            HasTransactions = Transactions.Count > 0;
            await LoadTransactionsAsync();
        }
    }

    [RelayCommand]
    public async Task AddNewTransactionAsync()
    {
        await Shell.Current.GoToAsync("AddEditTransactionPage");
    }

    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SelectedTypeFilter = "All";
        SelectedCategoryFilter = "All Categories";
        SelectedPeriodFilter = DatePeriodFilter.All;
        SearchText = string.Empty;
        await LoadTransactionsAsync();
    }
}
