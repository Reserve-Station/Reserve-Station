// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Traits.Components;

/// <summary>
///     A component that randomly varies user's mood. Triggers each time mood is changed.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MercurialComponent : Component
{
    /// <summary>
    ///     The lower bounds for random mood offsets.
    /// </summary>
    [DataField]
    public float LowerMood = -10f;

    /// <summary>
    ///     The upper bounds for random mood offsets.
    /// </summary>
    [DataField]
    public float UpperMood = 10f;
}
