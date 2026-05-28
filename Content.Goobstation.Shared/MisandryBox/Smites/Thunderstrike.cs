// SPDX-FileCopyrightText: 2025 Conchelle <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading;
using Content.Goobstation.Shared.MisandryBox.JumpScare;
using Content.Shared.Electrocution;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Goobstation.Shared.MisandryBox.Smites;

public sealed class ThunderstrikeSystem : EntitySystem
{
    [Dependency] private readonly IFullScreenImageJumpscare _jumpscare = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedElectrocutionSystem _elect = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    private const string Sound = "/Audio/_Goobstation/Effects/Smites/Thunderstrike/thunderstrike.ogg";
    private const string God = "/Textures/_Goobstation/MisandryBox/For he does not need no fucking rsi.png";

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
    }

    // efcc go get u alaye...
    public void Smite(EntityUid mumu, bool kill = true, TransformComponent? transform = null)
    {
        if (!Resolve(mumu, ref transform))
            return;

        CreateLighting(transform.Coordinates);

        _elect.TryDoElectrocution(mumu, null, 250, TimeSpan.FromSeconds(1), false, ignoreInsulation: true);

        if (!kill || !_player.TryGetSessionByEntity(mumu, out var sesh))
            return;

        var text = new SpriteSpecifier.Texture(new ResPath(God));
        _jumpscare.Jumpscare(text, sesh);

        QueueDel(mumu);
        Spawn("Ash", transform.Coordinates);
        _popup.PopupEntity(Loc.GetString("admin-smite-turned-ash-other", ("name", mumu)), mumu, PopupType.LargeCaution);
    }

    public void CreateLighting(EntityCoordinates coordinates)
    {
        var randomInt = _random.Next(1, 3); // two variants of lightning
        var coords = new EntityCoordinates(coordinates.EntityId, coordinates.Position.X + 0.5f, coordinates.Position.Y + 1.5f);
        var ent = Spawn($"EffectLightning{randomInt}", coords);

        var sound = new SoundPathSpecifier(Sound);
        _audio.PlayPvs(sound, coordinates, AudioParams.Default.WithVolume(150f));
    }
}
