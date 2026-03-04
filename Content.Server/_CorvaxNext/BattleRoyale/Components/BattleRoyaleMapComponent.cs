// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameObjects;

namespace Content.Server._CorvaxNext.BattleRoyale.Components;

/// <summary>
/// Marker component for Battle Royale map, should be added on gameMap prototype.
/// </summary>
[RegisterComponent]
public sealed partial class BattleRoyaleMapComponent : Component
{
    /// <summary>
    /// Clears <see cref="ImplicitRoofComponent"/> for all grids on that map
    /// </summary>
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public bool ClearImplicitRoofComponent = false;
}
