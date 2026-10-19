// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Network;
using Content.Goobstation.Shared.Xenobiology.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Timing;

namespace Content.Goobstation.Shared.Xenobiology.Systems;

// This handles mob growth between development stages.
public sealed partial class MobGrowthSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private HungerSystem _hunger = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private MobStateSystem _mobState = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MobGrowthComponent, ComponentInit>(OnMobGrowthInit);
    }

    private void OnMobGrowthInit(Entity<MobGrowthComponent> ent, ref ComponentInit args)
    {
        ent.Comp.NextGrowthTime = _timing.CurTime + ent.Comp.GrowthInterval;
        ent.Comp.BaseEntityName = Name(ent);

        if (!ent.Comp.Stages.ContainsKey(ent.Comp.CurrentStage))
        {
            Log.Error($"Invalid initial stage {ent.Comp.CurrentStage} for entity {ToPrettyString(ent)}");
            ent.Comp.CurrentStage = ent.Comp.FirstStage;
        }

        UpdateAppearance(ent);
    }

    // Checks entity hunger thresholds, if the threshold required by MobGrowth is met -> grow.
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var now = _timing.CurTime;
        var query = EntityQueryEnumerator<MobGrowthComponent, HungerComponent>();
        while (query.MoveNext(out var uid, out var growth, out var hunger))
        {
            if (now < growth.NextGrowthTime)
                continue;

            growth.NextGrowthTime = now + growth.GrowthInterval;

            if (_mobState.IsDead(uid)
                || _hunger.GetHunger(hunger) <= growth.HungerRequired
                || !growth.Stages.TryGetValue(growth.CurrentStage, out var currentData)
                || string.IsNullOrEmpty(currentData.NextStage))
                continue;

            DoGrowth((uid, growth, hunger));
        }
    }

    #region Helpers

    // Fairly barebones at the moment, this could be expanded to increase HP etc...
    private void DoGrowth(Entity<MobGrowthComponent, HungerComponent> ent)
    {
        var (uid, growth, hunger) = ent;

        if (TerminatingOrDeleted(ent))
            return;

        if (!growth.Stages.TryGetValue(growth.CurrentStage, out var currentStageData))
        {
            Log.Error($"Missing stage data for {growth.CurrentStage} on entity {ToPrettyString(uid)}");
            return;
        }

        if (currentStageData.NextStage is not { } nextStage ||
            !growth.Stages.ContainsKey(nextStage))
        {
            Log.Error($"Invalid next stage {currentStageData.NextStage} for entity {ToPrettyString(uid)}");
            return;
        }

        _hunger.ModifyHunger(uid, growth.GrowthCost, hunger);
        growth.CurrentStage = nextStage;
        Dirty(uid, growth);

        UpdateAppearance((uid, growth));
    }

    private void UpdateAppearance(Entity<MobGrowthComponent> ent)
    {
        if (!ent.Comp.Stages.TryGetValue(ent.Comp.CurrentStage, out var stageData)
            || !TryComp<AppearanceComponent>(ent, out var appearance)
            || stageData.Sprite is not { } sprite)
            return;

        _appearance.SetData(ent, GrowthStateVisuals.Sprite, sprite, appearance);

        if (_net.IsServer)
        {
            var stageName = stageData.DisplayName is { } display ? Loc.GetString(display) : string.Empty;
            _metaData.SetEntityName(ent, Loc.GetString("mob-growth-name", ("stage", stageName), ("name", ent.Comp.BaseEntityName)));
        }
    }

    #endregion
}
