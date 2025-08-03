using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Server.Audio;
using Content.Server.MyStuff.Components;

namespace Content.Server.MyStuff.Systems;

public sealed class RandomTimedSoundSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    public override void Update(float frameTime)
    {
        foreach (var comp in EntityQuery<RandomTimedSoundComponent>())
        {
            // ✅ Защита от краша при savemap
            if (!comp.MapSavable && _timing.ApplyingState)
                continue;

            if (_timing.CurTime < comp.NextPlay)
                continue;

            if (!_protoMan.TryIndex<SoundCollectionPrototype>(comp.SoundCollection, out var collection))
                continue;

            var file = _random.Pick(collection.PickFiles);

            var audioParams = AudioParams.Default
                .WithVolume(comp.Volume)
                .WithMaxDistance(comp.Range);

            _audio.PlayPvs(file, comp.Owner, audioParams);

            var delay = _random.NextFloat(comp.IntervalMin, comp.IntervalMax);
            comp.NextPlay = _timing.CurTime + TimeSpan.FromSeconds(delay);
        }
    }
}

