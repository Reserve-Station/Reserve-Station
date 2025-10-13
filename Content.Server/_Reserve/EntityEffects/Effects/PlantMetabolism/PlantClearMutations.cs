// SPDX-FileCopyrightText: 2025 Kutosss <162154227+Kutosss@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sa1nt7331 <202271576+sa1nt7331@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Botany.Components;
using Content.Shared.EntityEffects;
using Robust.Shared.Random;
using System.Collections.Generic;

namespace Content.Server.EntityEffects.Effects.PlantMetabolism;

public sealed partial class PlantClearMutations : PlantAdjustAttribute
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-mutations";

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (!CanMetabolize(args.TargetEntity, out var plantHolderComp, args.EntityManager))
            return;
        plantHolderComp.MutationLevel = 0;
        var seed = plantHolderComp.Seed!;
        var random = IoCManager.Resolve<IRobustRandom>();
        var actions = new List<System.Action?> 
        {
            seed.NutrientConsumption != 0.75f ? () => seed.NutrientConsumption = 0.75f : null,
            seed.WaterConsumption != 0.5f ? () => seed.WaterConsumption = 0.5f : null,
            seed.IdealHeat != 293f ? () => seed.IdealHeat = 293f : null,
            seed.HeatTolerance != 10f ? () => seed.HeatTolerance = 10f : null,
            seed.LowPressureTolerance != 81f ? () => seed.LowPressureTolerance = 81f : null,
            seed.HighPressureTolerance != 121f ? () => seed.HighPressureTolerance = 121f : null,
            seed.ToxinsTolerance != 4f ? () => seed.ToxinsTolerance = 4f : null,
            seed.PestTolerance != 5f ? () => seed.PestTolerance = 5f : null,
            seed.WeedTolerance != 5f ? () => seed.WeedTolerance = 5f : null,
            seed.Endurance != 100f ? () => seed.Endurance = 100f : null,
            seed.Maturation != 6f ? () => seed.Maturation = 6f : null,
            seed.Production != 6f ? () => seed.Production = 6f : null,
            seed.Lifespan != 60f ? () => seed.Lifespan = 60f : null,
            seed.Yield != 6 ? () => seed.Yield = 6 : null,
            seed.Potency != 50f ? () => seed.Potency = 50f : null,
            seed.Seedless != false ? () => seed.Seedless = false : null,
            seed.Ligneous != false ? () => seed.Ligneous = false : null,
            seed.TurnIntoKudzu != false ? () => seed.TurnIntoKudzu = false : null,
            seed.CanScream != false ? () => seed.CanScream = false : null,
        }.OfType<System.Action>().ToList();
        if (actions.Count > 0)
            random.Pick(actions).Invoke();
        var removableMutations = seed.Mutations.Where(m => m.Name != "PlantMutationInviable" && m.Name != "Inviable").ToList();
        if (removableMutations.Count > 0)
            seed.Mutations.Remove(random.Pick(removableMutations));
        if (seed.ExudeGasses.Count > 0)
            seed.ExudeGasses.Remove(random.Pick(seed.ExudeGasses.Keys.ToList()));
    }
}