using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceTracker.Models;
using FinanceTracker.Services;

namespace FinanceTracker.ViewModels;

[QueryProperty(nameof(TransactionId), "id")]
[QueryProperty(nameof(InitialType), "type")]
public partial class AddEditTransactionViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    public partial int TransactionId { get; set; }

    [ObservableProperty]
    public partial string InitialType { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FormTitle { get; set; } = "New Transaction";

    [ObservableProperty]
    public partial bool IsEditMode { get; set; }

    [ObservableProperty]
    public partial string RecordTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string AmountText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial TransactionType SelectedType { get; set; } = TransactionType.Expense;

    [ObservableProperty]
    public partial CategoryItem? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial PaymentMethod SelectedPaymentMethod { get; set; } = PaymentMethod.CreditCard;

    [ObservableProperty]
    public partial DateTime TransactionDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial string? Note { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasError { get; set; }

    public ObservableCollection<CategoryItem> AvailableCategories { get; } = new();
    public ObservableCollection<PaymentMethod> AvailablePaymentMethods { get; } = new();

    public AddEditTransactionViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Record Transaction";

        foreach (PaymentMethod method in Enum.GetValues<PaymentMethod>())
        {
            AvailablePaymentMethods.Add(method);
        }
    }

    async partial void OnTransactionIdChanged(int value)
    {
        if (value > 0)
        {
            IsEditMode = true;
            FormTitle = "Edit Transaction";
            await LoadExistingTransactionAsync(value);
        }
        else
        {
            IsEditMode = false;
            FormTitle = "New Transaction";
            await LoadCategoriesForTypeAsync();
        }
    }

    async partial void OnInitialTypeChanged(string value)
    {
        if (!IsEditMode && !string.IsNullOrWhiteSpace(value))
        {
            if (value.Equals("Income", StringComparison.OrdinalIgnoreCase))
            {
                SelectedType = TransactionType.Income;
            }
            else
            {
                SelectedType = TransactionType.Expense;
            }
            await LoadCategoriesForTypeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        if (TransactionId == 0)
        {
            await LoadCategoriesForTypeAsync();
        }
    }

    private async Task LoadExistingTransactionAsync(int id)
    {
        try
        {
            IsBusy = true;
            var item = await _dataService.GetTransactionByIdAsync(id);
            if (item != null)
            {
                RecordTitle = item.Title;
                AmountText = item.Amount.ToString("0.00", CultureInfo.InvariantCulture);
                SelectedType = item.Type;
                TransactionDate = item.TransactionDate;
                SelectedPaymentMethod = item.PaymentMethod;
                Note = item.Note;

                await LoadCategoriesForTypeAsync();
                SelectedCategory = AvailableCategories.FirstOrDefault(c => c.Name == item.Category);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCategoriesForTypeAsync()
    {
        var cats = await _dataService.GetCategoriesAsync(SelectedType);
        AvailableCategories.Clear();
        foreach (var c in cats)
        {
            AvailableCategories.Add(c);
        }

        if (SelectedCategory == null || SelectedCategory.Type != SelectedType)
        {
            SelectedCategory = AvailableCategories.FirstOrDefault();
        }
    }

    [RelayCommand]
    public async Task ChangeTypeAsync(string typeStr)
    {
        if (typeStr.Equals("Income", StringComparison.OrdinalIgnoreCase))
        {
            SelectedType = TransactionType.Income;
        }
        else
        {
            SelectedType = TransactionType.Expense;
        }

        await LoadCategoriesForTypeAsync();
    }

    [RelayCommand]
    public void SelectCategory(CategoryItem? category)
    {
        if (category != null)
        {
            SelectedCategory = category;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        // Validation
        if (string.IsNullOrWhiteSpace(RecordTitle))
        {
            HasError = true;
            ErrorMessage = "Please enter a transaction title or payee name.";
            return;
        }

        if (string.IsNullOrWhiteSpace(AmountText) ||
            (!decimal.TryParse(AmountText, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) &&
             !decimal.TryParse(AmountText, out amount)))
        {
            HasError = true;
            ErrorMessage = "Please enter a valid monetary amount.";
            return;
        }

        if (amount <= 0)
        {
            HasError = true;
            ErrorMessage = "Transaction amount must be greater than zero.";
            return;
        }

        if (SelectedCategory == null)
        {
            HasError = true;
            ErrorMessage = "Please select a category.";
            return;
        }

        try
        {
            IsBusy = true;

            var transaction = new TransactionItem
            {
                Id = TransactionId,
                Title = RecordTitle.Trim(),
                Amount = Math.Round(amount, 2),
                Type = SelectedType,
                Category = SelectedCategory.Name,
                CategoryIcon = SelectedCategory.Icon,
                CategoryColor = SelectedCategory.ColorHex,
                TransactionDate = TransactionDate,
                PaymentMethod = SelectedPaymentMethod,
                Note = Note?.Trim()
            };

            await _dataService.SaveTransactionAsync(transaction);

            WeakReferenceMessenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage("TransactionSaved"));

            // Pop back to previous page
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to save: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
