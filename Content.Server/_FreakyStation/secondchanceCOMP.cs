// SPDX-FileCopyrightText: 2025 ANIvarmin <danilter2021@gmail.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Freakystation;


[RegisterComponent]
public sealed partial class SecondChanceComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Brr = "Brr brr patapim";
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int Uses = 1;
}
