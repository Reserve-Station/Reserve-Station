using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.Brainrot;

[Serializable, NetSerializable]
public sealed class SyncBrainrotTriggersMessage : EntityEventArgs
{
    public List<string> Triggers { get; }

    public SyncBrainrotTriggersMessage(List<string> triggers)
    {
        Triggers = triggers;
    }
}
