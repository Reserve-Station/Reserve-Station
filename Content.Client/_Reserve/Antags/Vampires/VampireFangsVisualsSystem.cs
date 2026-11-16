// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Antags.Vampires;
using Content.Shared._Reserve.Antags.Vampires.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Reserve.Antags.Vampires;

/// <summary>
/// Draws bloody fangs on the vampire's face while the fangs are extended.
/// </summary>
public sealed class VampireFangsVisualsSystem : VisualizerSystem<VampireComponent>
{
    private static readonly SpriteSpecifier.Rsi FangsSprite =
        new(new ResPath("_Reserve/Vampire/fangs.rsi"), "fangs");

    protected override void OnAppearanceChange(EntityUid uid, VampireComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        AppearanceSystem.TryGetData<bool>(uid, VampireFangsVisuals.Extended, out var extended, args.Component);

        var sprite = (uid, args.Sprite);
        if (!SpriteSystem.LayerMapTryGet(sprite, VampireFangsVisuals.Layer, out var layer, false))
        {
            if (!extended)
                return;

            // Put the fangs over the face and beard, but under masks and helmets
            var index = SpriteSystem.LayerMapTryGet(sprite, HumanoidVisualLayers.FacialHair, out var facialHair, false)
                ? facialHair + 1
                : SpriteSystem.LayerMapTryGet(sprite, HumanoidVisualLayers.Head, out var head, false)
                    ? head + 1
                    : -1;

            layer = SpriteSystem.AddLayer(sprite, FangsSprite, index == -1 ? null : index);
            SpriteSystem.LayerMapSet(sprite, VampireFangsVisuals.Layer, layer);
        }

        SpriteSystem.LayerSetVisible(sprite, layer, extended);
    }
}
