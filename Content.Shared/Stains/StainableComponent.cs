// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Maths.FixedPoint; // Reserve - for FixedPoint2
using Content.Shared.Hands.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization; // Gaby
using Content.Shared.DoAfter;

namespace Content.Shared.Stains;

[RegisterComponent, NetworkedComponent]
public sealed partial class StainableComponent : Component
{
    [DataField]
    public string SolutionId = "stain";

    [DataField]
    public FixedPoint2 MaxVolume = FixedPoint2.New(5);

    [DataField]
    public FixedPoint2 StainVolume = 0.5f;

    // Reserve edit start: fire-nerf
    /// <summary>
    /// Share of a burning stain that becomes fire stacks on the wearer.
    /// Cloth is 1. Plated and sealed clothing uses less.
    /// </summary>
    [DataField]
    public float FireScale = 1f;
    // Reserve edit end: fire-nerf

    [DataField]
    public Dictionary<string, List<PrototypeLayerData>> ClothingVisuals = new();

    [DataField]
    public Dictionary<HandLocation, List<PrototypeLayerData>> ItemVisuals = new();

    [DataField]
    public List<PrototypeLayerData> IconVisuals = new();

    [ViewVariables]
    public HashSet<int> RevealedIconVisuals = new();

    [DataField]
    public float CleanseDelay = 15f;
}

[Serializable, NetSerializable]
public sealed partial class WringStainDoAfterEvent : SimpleDoAfterEvent
{
}
