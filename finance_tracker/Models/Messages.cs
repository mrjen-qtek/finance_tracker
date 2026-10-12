using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinanceTracker.Models;

public class SwitchTabMessage : ValueChangedMessage<int>
{
    public SwitchTabMessage(int targetTabIndex) : base(targetTabIndex)
    {
    }
}

public class RefreshDataMessage : ValueChangedMessage<string>
{
    public RefreshDataMessage(string reason = "") : base(reason)
    {
    }
}
