using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.ChatTrigger;

[Serializable, NetSerializable]
public sealed class SyncBrainrotTriggersMessage : EntityEventArgs
{
    public List<string> Triggers { get; }

    public SyncBrainrotTriggersMessage(List<string> triggers)
    {
        Triggers = triggers;
    }
}
