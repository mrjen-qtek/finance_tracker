# Personal Finance Tracking Mobile Application (.NET MAUI)

A modern, production-grade cross-platform mobile application developed with **.NET MAUI** adhering strictly to **MVVM (Model-View-ViewModel)** architecture and Clean Architecture principles.

---

## 🌟 Key Features

1. **Dashboard & Financial Health Monitoring**:
   - **Hero Balance Card**: Real-time Net Balance, Total Income, Total Expenses, and Savings Rate.
   - **Period Filtering**: Quick toggle between *Today*, *This Week*, *This Month*, and *All Time*.
   - **Spending Breakdown**: Dynamic top-category breakdown with visual percentage progress bars.
   - **Recent Activity**: Quick snapshot of latest transactions with direct tap-to-edit navigation.
   - **Demo Data Seed & Reset**: Pre-populated with realistic personal finance data and one-tap restore capability.

2. **Income & Expense Management**:
   - Record and categorize income (Salary, Freelance, Dividends, etc.) and expenses (Groceries, Housing, Dining, Utilities, Transportation, etc.).
   - Support for payment methods (*Credit Card*, *Debit Card*, *Bank Transfer*, *Cash*, *UPI / Mobile*).
   - Form input validation (title requirement, positive numeric amounts, category selection).
   - Category badges with icons and custom color themes.

3. **Transaction History & Filter Engine**:
   - Instant search by title, category name, or personal notes.
   - Multi-dimensional filters: Type (*All*, *Expenses*, *Income*), Category dropdown, and Time period.
   - Aggregated metrics header displaying live count and sum of filtered results.
   - Swipe gestures (`SwipeView`) for quick deletion, plus tap-to-edit.
   - Pull-to-refresh (`RefreshView`).

4. **Analytics & Budget Tracking**:
   - **Category Distribution**: Detailed expenditure breakdown with percentages and counts.
   - **Monthly Trends**: Comparative paired bar charts tracking Income vs Expense across past months.
   - **Budget Limit Monitoring**: Real-time spending versus monthly budget targets with color-coded safety thresholds:
     - 🟢 **Normal**: < 85% spent
     - 🟡 **Warning**: 85% - 100% spent
     - 🔴 **Over Budget**: > 100% spent

---

## 🏗️ Architecture & Technology Stack

- **Framework**: .NET MAUI (.NET 10)
- **Architecture Pattern**: MVVM (Model - View - ViewModel)
- **MVVM Toolkit**: `CommunityToolkit.Mvvm` (using C# partial properties and source-generated RelayCommands)
- **Data Persistence**: `sqlite-net-pcl` asynchronous local SQLite database with thread-safe locking (`SemaphoreSlim`)
- **Dependency Injection**: Registered in `MauiProgram.cs` with clear lifetime scopes (Singleton Data Services, Transient ViewModels & Views)
- **Navigation**: MAUI AppShell with modern TabBar and route-based navigation (`AddEditTransactionPage`)
- **Unit Testing**: xUnit test project covering business calculations, model conversions, and budget thresholds

### Directory Structure

```text
FinanceTracker/
├── Converters/
│   └── Converters.cs                     # XAML Value Converters (Currency, Colors, Percentages, InverseBool)
├── Models/
│   ├── Enums.cs                          # TransactionType, PaymentMethod, DatePeriodFilter
│   ├── CategoryItem.cs                   # Category entity with SQLite attributes & budgets
│   ├── TransactionItem.cs                # Transaction entity with SQLite attributes & presentation bindings
│   ├── FinancialSummary.cs               # Financial metrics DTO (Income, Expense, Balance, Savings Rate)
│   ├── CategorySummary.cs                # Category spending & budget status DTO
│   └── MonthlyTrendItem.cs               # Paired monthly income/expense chart DTO
├── Services/
│   ├── IDataService.cs                   # Async repository contract
│   ├── SqliteDataService.cs              # Thread-safe SQLite database service
│   └── SeedData.cs                       # Realistic financial starter data
├── ViewModels/
│   ├── BaseViewModel.cs                  # Shared observable state & busy indicators
│   ├── DashboardViewModel.cs             # Overview metrics & quick actions
│   ├── TransactionsViewModel.cs          # Search, filters, swipe-delete & list logic
│   ├── AddEditTransactionViewModel.cs    # Add/Edit form validation & state
│   └── AnalyticsViewModel.cs             # Category breakdown, trends & budget tracking
├── Views/
│   ├── DashboardPage.xaml (.cs)          # Main dashboard view
│   ├── TransactionsPage.xaml (.cs)       # Filterable transaction list view
│   ├── AddEditTransactionPage.xaml (.cs) # Clean transaction input form
│   └── AnalyticsPage.xaml (.cs)          # Financial analytics & budget view
├── Resources/
│   ├── Styles/
│   │   ├── Colors.xaml                   # Fintech color palette & tokens
│   │   └── Styles.xaml                   # Component styling & typography
└── tests/
    └── FinanceTracker.Tests/             # Unit test suite with xUnit
```

---

## 🚀 Running the Application

### Build the Application
```bash
dotnet build -f net10.0-windows10.0.19041.0
```

### Run on Windows Desktop
```bash
dotnet run -f net10.0-windows10.0.19041.0
```

### Run Unit Tests
```bash
dotnet test tests/FinanceTracker.Tests/FinanceTracker.Tests.csproj -f net10.0-windows10.0.19041.0
```

## Output

<img width="452" height="1030" alt="image" src="https://github.com/user-attachments/assets/4ea5290b-2dff-4a21-9cac-7d47b1ba98ff" />
