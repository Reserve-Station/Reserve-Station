using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.ChatTrigger;

[Prototype("brainrotBotTrigger")]
public sealed partial class BrainrotBotTriggerPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public string Trigger { get; private set; } = string.Empty;

    [DataField]
    public string? Replacement { get; private set; }

    [DataField(required: true)]
    public LocId Description { get; private set; }
}
