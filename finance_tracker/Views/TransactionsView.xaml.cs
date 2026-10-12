using FinanceTracker.ViewModels;

namespace FinanceTracker.Views;

public partial class TransactionsView : ContentView
{
    public TransactionsView()
    {
        InitializeComponent();
    }

    private async void OnCategoryPickerChanged(object? sender, EventArgs e)
    {
        if (BindingContext is TransactionsViewModel vm && sender is Picker picker && picker.SelectedItem is string selectedCategory)
        {
            await vm.ChangeCategoryFilterAsync(selectedCategory);
        }
    }
}
