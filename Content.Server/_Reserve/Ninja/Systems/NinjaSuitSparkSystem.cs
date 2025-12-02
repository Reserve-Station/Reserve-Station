

using Content.Server._Reserve.Ninja.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
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

    private static readonly EntProtoId SparkPrototype = "EffectSparks";

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

            sparkComp.NextSparkTime = currentTime + TimeSpan.FromSeconds(sparkComp.SparkInterval);
        }
    }
}
