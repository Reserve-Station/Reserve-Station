// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Antags.Vampires.Components;

/// <summary>
/// Makes a spawned decoy copy visual data from the owning vampire.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState: true)]
public sealed partial class VampireDecoyAppearanceComponent : Component
{
    /// <summary>
    /// The entity that should be visually duplicated.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Source;
}
