

using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Reserve.Ninja.Components;

/// <summary>
/// Component that makes the ninja suit emit sparks periodically while invisible.
/// </summary>
[RegisterComponent]
public sealed partial class NinjaSuitSparkComponent : Component
{
    [DataField]
    public float SparkInterval = 30f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextSparkTime = TimeSpan.Zero;

    [DataField]
    public SoundSpecifier SparkSound = new SoundCollectionSpecifier("sparks");
}
