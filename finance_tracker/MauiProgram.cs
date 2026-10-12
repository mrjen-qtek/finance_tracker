using FinanceTracker.Services;
using FinanceTracker.ViewModels;
using FinanceTracker.Views;
using Microsoft.Extensions.Logging;

namespace FinanceTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Data Persistence Layer
        builder.Services.AddSingleton<IDataService, SqliteDataService>();

        // Register MVVM ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<AddEditTransactionViewModel>();
        builder.Services.AddTransient<AnalyticsViewModel>();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<AddEditTransactionPage>();
        builder.Services.AddTransient<AnalyticsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
