// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Xenobiology.Components;

/// <summary>
/// Marker for mobs a xenovac can suck up besides slimes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class XenoVacTargetComponent : Component;
