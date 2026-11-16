using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameStates;

namespace Content.Client.Revenant;

[RegisterComponent, NetworkedComponent]
public sealed partial class RevenantAnimatedComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float Accumulator = 0f;

    [ViewVariables(VVAccess.ReadOnly)]
    public Entity<PointLightComponent>? LightOverlay;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Color LightColor = Color.MediumPurple;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float LightRadius = 2f;

    // Reserve edit start - hover animation
    /// <summary>
    /// How high above the floor the item hovers.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float FloatHeight = 0.15f;

    /// <summary>
    /// How far the item bobs up and down while hovering.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float FloatAmplitude = 0.1f;

    /// <summary>
    /// Seconds to go from the bottom of the bob to the top.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float FloatTime = 1f;

    /// <summary>
    /// Color of the outline drawn around the item. Its alpha pulses together with the light.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Color OutlineColor = Color.FromHex("#9B30FF");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float OutlineWidth = 1.5f;

    [ViewVariables(VVAccess.ReadOnly)]
    public ShaderInstance? Outline;
    // Reserve edit end
}
