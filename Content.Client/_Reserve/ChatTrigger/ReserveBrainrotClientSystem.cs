using Content.Shared._Reserve.ChatTrigger;

namespace Content.Client._Reserve.ChatTrigger;

public sealed class ReserveBrainrotClientSystem : EntitySystem
{
    private readonly List<string> _customTriggers = new();
    private readonly List<string[]> _parsedVariants = new();

    public IReadOnlyList<string> CustomTriggers => _customTriggers;
    public IReadOnlyList<string[]> ParsedVariants => _parsedVariants;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<SyncBrainrotTriggersMessage>(OnSyncTriggers);
    }

    private void OnSyncTriggers(SyncBrainrotTriggersMessage msg)
    {
        ChatTriggerHelper.ClearDynamicCache();
        _customTriggers.Clear();
        _parsedVariants.Clear();

        foreach (var trigger in msg.Triggers)
        {
            _customTriggers.Add(trigger);
            var variants = ChatTriggerHelper.ParseTriggerVariants(trigger);
            _parsedVariants.Add(variants);
            foreach (var variant in variants)
            {
                ChatTriggerHelper.Precompile(variant, isDynamic: true);
            }
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();
        ChatTriggerHelper.ClearDynamicCache();
    }
}
