// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

public sealed partial class PlantClearMutations : BasePlantAdjustAttribute<PlantClearMutations>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-mutations";

    public override bool GuidebookIsAttributePositive { get; protected set; } = false; // Reserve - Upstream 2408 fixes
}
