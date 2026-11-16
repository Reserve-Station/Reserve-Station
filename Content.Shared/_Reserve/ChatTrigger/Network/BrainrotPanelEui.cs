using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.ChatTrigger;

[NetSerializable, Serializable]
public sealed class BrainrotPanelEuiState : EuiStateBase
{
    public List<string> CustomTriggers { get; }

    public BrainrotPanelEuiState(List<string> customTriggers)
    {
        CustomTriggers = customTriggers;
    }
}

[NetSerializable, Serializable]
public sealed class AddBrainrotTriggerMsg : EuiMessageBase
{
    public string Trigger { get; }

    public AddBrainrotTriggerMsg(string trigger)
    {
        Trigger = trigger;
    }
}

[NetSerializable, Serializable]
public sealed class RemoveBrainrotTriggerMsg : EuiMessageBase
{
    public string Trigger { get; }

    public RemoveBrainrotTriggerMsg(string trigger)
    {
        Trigger = trigger;
    }
}
