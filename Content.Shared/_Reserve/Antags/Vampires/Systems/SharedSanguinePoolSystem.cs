// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Antags.Vampires.Components;
using Content.Shared.Physics;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Events;

namespace Content.Shared._Reserve.Antags.Vampires.Systems;

public abstract class SharedSanguinePoolSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SanguinePoolComponent, PreventCollideEvent>(OnPreventCollide);
    }

    private void OnPreventCollide(Entity<SanguinePoolComponent> ent, ref PreventCollideEvent args)
    {
        if (HasComp<MapGridComponent>(args.OtherEntity))
            return;

        var otherLayer = (CollisionGroup) args.OtherFixture.CollisionLayer;
        if ((otherLayer & CollisionGroup.WallLayer) != 0)
            return;

        args.Cancelled = true;
    }
}
