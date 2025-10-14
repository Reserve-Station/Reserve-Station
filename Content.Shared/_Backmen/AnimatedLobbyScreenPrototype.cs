using Robust.Shared.Prototypes;

namespace Content.Shared._Backmen;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class AnimatedLobbyScreenPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("path", required: true)]
    public string Path { get; private set; } = default!;
}
