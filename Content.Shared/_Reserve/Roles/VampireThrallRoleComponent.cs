// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared.Roles.Components;

/// <summary>
/// Mind role marker for Vampire thralls.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class VampireThrallRoleComponent : BaseMindRoleComponent;
