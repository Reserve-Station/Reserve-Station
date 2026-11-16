// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Antags.Vampires.Components.Classes;

/// <summary>
/// Marker component indicating Blood Swell is currently active
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ActiveBloodSwellComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan EndTime;
}
