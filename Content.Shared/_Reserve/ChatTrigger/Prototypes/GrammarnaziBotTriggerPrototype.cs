using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.ChatTrigger;

[Prototype("grammarnaziBotTrigger")]
public sealed partial class GrammarnaziBotTriggerPrototype : IPrototype
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
