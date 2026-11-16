// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Overlays;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SaturationScaleOverlayComponent : Component
{
    [DataField, AutoNetworkedField]
    public float NeutralMoodThreshold = 1f;

    [DataField, AutoNetworkedField]
    public float SaturationScale = 1f;

    /// <summary>
    ///     Modifies how quickly the saturation "fades in", normally at a rate of 1% per second times this multiplier.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float FadeInMultiplier = 0.1f;
}
