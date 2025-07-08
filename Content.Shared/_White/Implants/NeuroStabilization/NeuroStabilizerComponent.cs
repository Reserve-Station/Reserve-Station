// SPDX-FileCopyrightText: 2025 Neverluckz <yanechurka1000.7@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._White.Implants.NeuroStabilization;

[RegisterComponent]
public sealed partial class NeuroStabilizationComponent : Component
{
    [DataField]
    public bool Electrocution = true;

    [DataField]
    public TimeSpan TimeElectrocution = TimeSpan.FromSeconds(1);

    [DataField]
    public float DamageModifier = 0.66f;
}
