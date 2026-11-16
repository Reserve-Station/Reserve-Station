// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Antags.Vampires.Components;
using Robust.Client.GameObjects;

namespace Content.Client._Reserve.Antags.Vampires;

/// <summary>
/// Copies the vampire's current sprite onto its decoy, so the decoy looks like the vampire
/// instead of the placeholder sprite from the prototype.
/// </summary>
public sealed class VampireDecoyAppearanceSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VampireDecoyAppearanceComponent, AfterAutoHandleStateEvent>(OnAfterState);
    }

    private void OnAfterState(Entity<VampireDecoyAppearanceComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (ent.Comp.Source is not { } source
            || !TryComp<SpriteComponent>(source, out var sourceSprite)
            || !TryComp<SpriteComponent>(ent, out var decoySprite))
            return;

        _sprite.CopySprite((source, sourceSprite), (ent, decoySprite));
    }
}
