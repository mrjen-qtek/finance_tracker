using FinanceTracker.Views;

namespace FinanceTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddEditTransactionPage), typeof(AddEditTransactionPage));
    }
}
