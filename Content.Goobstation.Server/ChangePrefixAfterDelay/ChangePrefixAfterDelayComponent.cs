// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ted Lukin <66275205+pheenty@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pheenty <fedorlukin2006@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Server.ChangePrefixAfterDelay;

/// <summary>
/// Changes held and equipped prefix of an item after the delay, then removes itself.
/// </summary>
[RegisterComponent]
public sealed partial class ChangePrefixAfterDelayComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan? ChangeAt;

    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(0.3);

    [DataField]
    public string? NewHeldPrefix;

    [DataField]
    public string? NewEquippedPrefix;
}
