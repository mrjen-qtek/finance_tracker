using FinanceTracker.ViewModels;

namespace FinanceTracker.Views;

public partial class AddEditTransactionPage : ContentPage
{
    private readonly AddEditTransactionViewModel _viewModel;

    public AddEditTransactionPage(AddEditTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
