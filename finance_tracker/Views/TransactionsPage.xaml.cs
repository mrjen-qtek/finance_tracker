using FinanceTracker.ViewModels;

namespace FinanceTracker.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _viewModel;

    public TransactionsPage(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTransactionsAsync();
    }

    private async void OnCategoryPickerChanged(object? sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem is string selectedCategory)
        {
            await _viewModel.ChangeCategoryFilterAsync(selectedCategory);
        }
    }
}
