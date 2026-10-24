// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Content.Shared.EntityEffects;

namespace Content.Goobstation.Shared.Xenobiology;

/// <summary>
/// Applies entity effects when this status effect is applied or removed.
/// Trauma keeps the same component in Content.Trauma.Shared. Xenobiology extracts delay an explosion and metal foam through it.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class StatusEffectEffectsApplyComponent : Component
{
    /// <summary>
    /// The effects to apply when the status effect is applied.
    /// </summary>
    [DataField]
    public EntityEffect[]? EffectsOnApply;

    /// <summary>
    /// The effects to apply when the status effect is removed.
    /// </summary>
    [DataField]
    public EntityEffect[]? EffectsOnRemoval;
}
