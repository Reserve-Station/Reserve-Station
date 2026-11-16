// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Antags.Vampires.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class VampiricClawsComponent : Component
{
    /// <summary>
    /// How many successful melee hits before the claws dissipate
    /// </summary>
    [DataField]
    public int HitsRemaining = 15;

    [DataField]
    public int BloodPerHit = 5;
}
