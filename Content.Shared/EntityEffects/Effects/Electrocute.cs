// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects;
using Content.Shared.Electrocution;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

public sealed partial class Electrocute : EntityEffectBase<Electrocute>
{
    [DataField] public int ElectrocuteTime = 2;

    [DataField] public int ElectrocuteDamageScale = 5;

    /// <remarks>
    ///     true - refresh electrocute time,  false - accumulate electrocute time
    /// </remarks>
    [DataField] public bool Refresh = true;

    [DataField] public float ElectrocutionChance = 1f; // Reserve edit - ElectrocutionChance

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-electrocute", ("chance", Probability), ("time", ElectrocuteTime));
}

public sealed partial class ElectrocuteSystem : EntityEffectSystem<FixturesComponent, Electrocute>
{
    [Dependency] private readonly SharedElectrocutionSystem _electrocution = default!;

    protected override void Effect(Entity<FixturesComponent> entity, ref EntityEffectEvent<Electrocute> args)
    {
        var effect = args.Effect;
        _electrocution.TryDoElectrocution(entity, null,
            Math.Max((int) (args.Scale * effect.ElectrocuteDamageScale), 1),
            TimeSpan.FromSeconds(effect.ElectrocuteTime),
            effect.Refresh,
            electrocutionChance: effect.ElectrocutionChance,
            ignoreInsulation: true);
    }
}
