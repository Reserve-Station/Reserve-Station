// SPDX-FileCopyrightText: 2025 Kill_Me_I_Noobs <118206719+Vonsant@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * All right reserved to CrystallEdge.
 *
 * BUT this file is sublicensed under MIT License
 *
 */

using Robust.Shared.Utility;

namespace Content.Server._CorvaxNext.AdditionalMap;

/// <summary>
/// Loads additional maps from the list at the start of the round.
/// </summary>
[RegisterComponent, Access(typeof(StationAdditionalMapSystem))]
public sealed partial class StationAdditionalMapComponent : Component
{
    /// <summary>
    /// A map paths to load on a new map.
    /// </summary>
    [DataField]
    public List<ResPath> MapPaths = new();
}
