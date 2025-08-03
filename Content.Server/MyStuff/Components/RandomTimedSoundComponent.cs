using Robust.Shared.GameObjects;

namespace Content.Server.MyStuff.Components;

[RegisterComponent]
public sealed partial class RandomTimedSoundComponent : Component
{
    [DataField("soundCollection", required: true)]
    public string SoundCollection = string.Empty;

    [DataField("intervalMin")]
    public float IntervalMin = 30f;

    [DataField("intervalMax")]
    public float IntervalMax = 60f;

    [DataField("volume")]
    public float Volume = -4f;

    [DataField("range")]
    public float Range = 5f;

    [DataField("maxInstances")]
    public int MaxInstances = 1;

    // 🚫 запрет на сохранение при savemap
    [DataField("mapSavable")]
    public bool MapSavable = false;

    public TimeSpan NextPlay = TimeSpan.Zero;
}
