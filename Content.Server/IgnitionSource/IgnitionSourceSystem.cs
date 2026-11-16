// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Atmos.EntitySystems;
using Content.Shared.IgnitionSource;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;

namespace Content.Server.IgnitionSource;

public sealed partial class IgnitionSourceSystem : SharedIgnitionSourceSystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    // Reserve edit start: fire-nerf
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedEntityStorageSystem _entityStorage = default!;
    // Reserve edit end: fire-nerf

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<IgnitionSourceComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (!comp.Ignited)
                continue;

            // Reserve edit start: fire-nerf
            if (IsFlameSealed(uid))
                continue;
            // Reserve edit end: fire-nerf

            if (xform.GridUid is { } gridUid)
            {
                var position = _transform.GetGridOrMapTilePosition(uid, xform);
                // TODO: Should this be happening every single tick?
                _atmosphere.HotspotExpose(gridUid, position, comp.Temperature, 50, uid, true);
            }
        }
    }

    // Reserve edit start: fire-nerf
    private bool IsFlameSealed(EntityUid uid)
    {
        var current = uid;
        while (_container.TryGetContainingContainer(current, out var container))
        {
            if (TryComp<EntityStorageComponent>(container.Owner, out var storage))
            {
                if (!_entityStorage.IsOpen(container.Owner, storage))
                    return true;
            }
            else if (container.OccludesLight)
            {
                return true;
            }

            current = container.Owner;
        }

        return false;
    }
    // Reserve edit end: fire-nerf
}