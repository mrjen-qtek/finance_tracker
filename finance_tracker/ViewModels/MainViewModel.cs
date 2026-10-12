using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceTracker.Models;
using FinanceTracker.Views;

namespace FinanceTracker.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public DashboardViewModel DashboardVm { get; }
    public TransactionsViewModel TransactionsVm { get; }
    public AnalyticsViewModel AnalyticsVm { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDashboardActive))]
    [NotifyPropertyChangedFor(nameof(IsTransactionsActive))]
    [NotifyPropertyChangedFor(nameof(IsAnalyticsActive))]
    [NotifyPropertyChangedFor(nameof(DashboardTabBg))]
    [NotifyPropertyChangedFor(nameof(TransactionsTabBg))]
    [NotifyPropertyChangedFor(nameof(AnalyticsTabBg))]
    [NotifyPropertyChangedFor(nameof(DashboardTabTextColor))]
    [NotifyPropertyChangedFor(nameof(TransactionsTabTextColor))]
    [NotifyPropertyChangedFor(nameof(AnalyticsTabTextColor))]
    public partial int SelectedTabIndex { get; set; } = 0;

    public bool IsDashboardActive => SelectedTabIndex == 0;
    public bool IsTransactionsActive => SelectedTabIndex == 1;
    public bool IsAnalyticsActive => SelectedTabIndex == 2;

    public Color DashboardTabBg => IsDashboardActive ? Color.FromArgb("#1E40AF") : Colors.Transparent;
    public Color TransactionsTabBg => IsTransactionsActive ? Color.FromArgb("#1E40AF") : Colors.Transparent;
    public Color AnalyticsTabBg => IsAnalyticsActive ? Color.FromArgb("#1E40AF") : Colors.Transparent;

    public Color DashboardTabTextColor => IsDashboardActive ? Colors.White : Color.FromArgb("#BFDBFE");
    public Color TransactionsTabTextColor => IsTransactionsActive ? Colors.White : Color.FromArgb("#BFDBFE");
    public Color AnalyticsTabTextColor => IsAnalyticsActive ? Colors.White : Color.FromArgb("#BFDBFE");

    public MainViewModel(
        DashboardViewModel dashboardVm,
        TransactionsViewModel transactionsVm,
        AnalyticsViewModel analyticsVm)
    {
        DashboardVm = dashboardVm;
        TransactionsVm = transactionsVm;
        AnalyticsVm = analyticsVm;

        Title = "Personal Finance";

        WeakReferenceMessenger.Default.Register<SwitchTabMessage>(this, async (r, m) =>
        {
            await SelectTabAsync(m.Value);
        });

        WeakReferenceMessenger.Default.Register<RefreshDataMessage>(this, async (r, m) =>
        {
            await RefreshActiveTabAsync();
        });
    }

    [RelayCommand]
    public async Task SelectTabAsync(object? param)
    {
        int index = 0;
        if (param is int i)
            index = i;
        else if (param is string s && int.TryParse(s, out int parsed))
            index = parsed;

        SelectedTabIndex = index;
        await RefreshActiveTabAsync();
    }

    [RelayCommand]
    public async Task NavigateToAddAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddEditTransactionPage));
    }

    public async Task RefreshActiveTabAsync()
    {
        switch (SelectedTabIndex)
        {
            case 0:
                await DashboardVm.LoadDashboardDataAsync();
                break;
            case 1:
                await TransactionsVm.LoadTransactionsAsync();
                break;
            case 2:
                await AnalyticsVm.LoadAnalyticsAsync();
                break;
        }
    }
}
