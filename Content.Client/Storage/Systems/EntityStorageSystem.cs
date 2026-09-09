// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;

namespace Content.Client.Storage.Systems;

public sealed class EntityStorageSystem : SharedEntityStorageSystem
{
    public override bool ResolveStorage(EntityUid uid, [NotNullWhen(true)] ref EntityStorageComponent? component)
    {
        if (component != null)
            return true;

        TryComp<EntityStorageComponent>(uid, out var storage);
        component = storage;
        return component != null;
    }
}
