

using Content.Server._Reserve.Ninja.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Reserve.Ninja.Systems;

/// <summary>
/// Handles spark effects for ninja suit while invisible.
/// </summary>
public sealed class NinjaSuitSparkSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private static readonly EntProtoId SparkPrototype = "EffectSparks";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NinjaSuitSparkComponent, ItemToggledEvent>(OnSuitToggled);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var currentTime = _timing.CurTime;
        var query = EntityQueryEnumerator<NinjaSuitSparkComponent, ItemToggleComponent>();
        
        while (query.MoveNext(out var uid, out var sparkComp, out var toggleComp))
        {
            // Only spark if the suit is activated (invisible)
            if (!toggleComp.Activated)
                continue;

            // Check if it's time to spark
            if (currentTime < sparkComp.NextSparkTime)
                continue;

            if (!_inventory.TryGetContainingEntity(uid, out var wearer))
                continue;

            var coords = Transform(wearer.Value).Coordinates;
            Spawn(SparkPrototype, coords);
            _audio.PlayPvs(sparkComp.SparkSound, wearer.Value);

            sparkComp.NextSparkTime = currentTime + TimeSpan.FromSeconds(sparkComp.SparkInterval);
        }
    }

    private void OnSuitToggled(Entity<NinjaSuitSparkComponent> ent, ref ItemToggledEvent args)
    {
        var (uid, comp) = ent;
        
        if (args.Activated)
        {
            comp.NextSparkTime = _timing.CurTime + TimeSpan.FromSeconds(comp.SparkInterval);
        }
        else
        {
            comp.NextSparkTime = TimeSpan.Zero;
        }
    }
}
