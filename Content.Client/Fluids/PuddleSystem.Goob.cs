using Content.Client.IconSmoothing;
using Content.Goobstation.Common.Footprints;
using Content.Shared._ES.Fluids;
using Content.Shared.Fluids;
using Content.Shared.Random.Helpers;
using Robust.Client.GameObjects;
using Robust.Shared.Random;

namespace Content.Client.Fluids;

public sealed partial class PuddleSystem
{
    [Dependency] private readonly IconSmoothSystem _smooth = default!; // reserve
    private void UpdatePuddleSprite(Entity<SpriteComponent> ent, float volume, IReadOnlyDictionary<Enum, object> appearanceData)
    {
        if (HasComp<FootprintComponent>(ent))
        {
            if (appearanceData.TryGetValue(PuddleVisuals.SolutionColor, out var footprintColor))
                _sprite.SetColor(ent.AsNullable(), ((Color) footprintColor).WithAlpha(0.7f));
            return;
        }

        // Reserve start - revert puddle visuals
        _sprite.LayerSetRotation(ent.AsNullable(), 0, Angle.Zero);

        // Update smoothing and sprite based on volume.
        if (TryComp<IconSmoothComponent>(ent, out var smooth))
        {
            if (volume < LowThreshold)
            {
                _sprite.LayerSetRsiState(ent.AsNullable(), 0, $"{smooth.StateBase}a");
                _smooth.SetEnabled(ent, false, smooth);
            }
            else if (volume < MediumThreshold)
            {
                _sprite.LayerSetRsiState(ent.AsNullable(), 0, $"{smooth.StateBase}b");
                _smooth.SetEnabled(ent, false, smooth);
            }
            else
            {
                if (!smooth.Enabled)
                {
                    _sprite.LayerSetRsiState(ent.AsNullable(), 0, $"{smooth.StateBase}0");
                    _smooth.SetEnabled(ent, true, smooth);
                    _smooth.DirtyNeighbours(ent, smooth);
                }
            }
        }

        var baseColor = Color.White;

        if (appearanceData.TryGetValue(PuddleVisuals.SolutionColor, out var colorObj))
        {
            var color = (Color) colorObj;
            _sprite.SetColor(ent.AsNullable(), color * baseColor);
        }
        else
        {
            _sprite.SetColor(ent.AsNullable(), ent.Comp.Color * baseColor);
        }
        // Reserve end
    }
}
