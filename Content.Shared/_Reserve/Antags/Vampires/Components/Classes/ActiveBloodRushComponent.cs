// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Antags.Vampires.Components.Classes;

/// <summary>
/// Marker component indicating Blood Rush is currently active
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ActiveBloodRushComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan EndTime;
}
