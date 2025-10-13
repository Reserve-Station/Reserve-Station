// SPDX-FileCopyrightText: 2025 BombasterDS2 <bombasterds.github@mail.ru>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Alert.Components;

/// <summary>
/// Generic component for alerts that have needs to update when some value in some component changes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ValueRelatedAlertComponent : Component
{
    [DataField]
    public short MaxSeverity = 0;

    [DataField]
    public string IconPrefix = "";
}
