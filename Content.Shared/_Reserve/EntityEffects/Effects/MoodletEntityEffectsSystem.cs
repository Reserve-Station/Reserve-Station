// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Mood;
using Content.Shared.EntityEffects;

namespace Content.Shared._Reserve.EntityEffects.Effects;

public sealed class ChemAddMoodletSystem : EntityEffectSystem<MetaDataComponent, ChemAddMoodlet>
{
    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<ChemAddMoodlet> args)
    {
        var ev = new MoodEffectEvent(args.Effect.MoodPrototype);
        RaiseLocalEvent(entity.Owner, ev);
    }
}

public sealed class ChemPurgeMoodletsSystem : EntityEffectSystem<MetaDataComponent, ChemPurgeMoodlets>
{
    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<ChemPurgeMoodlets> args)
    {
        var ev = new MoodPurgeEffectsEvent(args.Effect.RemovePermanentMoodlets);
        RaiseLocalEvent(entity.Owner, ev);
    }
}

public sealed class ChemRemoveMoodletSystem : EntityEffectSystem<MetaDataComponent, ChemRemoveMoodlet>
{
    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<ChemRemoveMoodlet> args)
    {
        var ev = new MoodRemoveEffectEvent(args.Effect.MoodPrototype);
        RaiseLocalEvent(entity.Owner, ev);
    }
}
