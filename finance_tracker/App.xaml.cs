namespace FinanceTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell())
        {
            Title = "Personal Finance Tracker",
            Width = 430,
            Height = 860,
            MinimumWidth = 380,
            MinimumHeight = 700
        };

        return window;
    }
}