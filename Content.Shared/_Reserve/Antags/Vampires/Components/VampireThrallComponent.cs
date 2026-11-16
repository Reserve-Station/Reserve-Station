// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Chemistry.Reagent;
using Content.Goobstation.Maths.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.Antags.Vampires.Components;

/// <summary>
///     Marker component applied to entities that have been enthralled by a Dantalion vampire.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class VampireThrallComponent : Component
{
    /// <summary>
    ///     The vampire currently controlling this thrall
    /// </summary>
    [AutoNetworkedField]
    public EntityUid? Master;

    [DataField]
    public FixedPoint2 HolyWaterConsumed = FixedPoint2.Zero;
    [DataField]
    public FixedPoint2 HolyWaterToBreakFree = FixedPoint2.New(30);
    [DataField]
    public ProtoId<ReagentPrototype> HolyWaterReagentId = "Holywater";
}
