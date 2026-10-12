using FinanceTracker.ViewModels;

namespace FinanceTracker.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.DashboardVm.LoadDashboardDataAsync();
        await _viewModel.TransactionsVm.LoadTransactionsAsync();
        await _viewModel.AnalyticsVm.LoadAnalyticsAsync();
    }
}
