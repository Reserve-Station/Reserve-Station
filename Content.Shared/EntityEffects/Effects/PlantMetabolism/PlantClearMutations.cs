// SPDX-FileCopyrightText: 2025 Kutosss <162154227+Kutosss@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sa1nt7331 <202271576+sa1nt7331@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects.Effects.PlantMetabolism;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed partial class PlantClearMutations : PlantAdjustAttribute<PlantClearMutations>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-mutations";
}
