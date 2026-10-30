// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Server.PlayerListener;

[RegisterComponent]
public sealed partial class DormNotifierMarkerComponent : Component
{
    [DataField]
    public string Name = "dorm-notifier-default";  // Reserve edit: Localization

    /// <summary>
    /// Tile range to check for players
    /// </summary>
    [DataField]
    public float ProximityRadius = 1;
}
