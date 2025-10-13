// SPDX-FileCopyrightText: 2025 JohnJohn <189290423+JohnJJohn@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.HellGoose.Components;

/// <summary>
/// Component for the hell goose teleporting back to the station
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HellGooseTeleportComponent : Component;
