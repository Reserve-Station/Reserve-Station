// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.Trail;

/// <summary>
/// Data definition for configuring a motion trail on any entity.
/// </summary>
[DataDefinition, Serializable, NetSerializable] // Reserve
public sealed partial class TrailSettings
{
    [DataField]
    public Color Color = Color.White;

    [DataField]
    public int MaxPoints = 64;

    [DataField]
    public float LineWidth = 0.65f;

    [DataField]
    public float MinDistance = 0.12f;

    /// <summary>Seconds before idle trail starts decaying.</summary>
    [DataField]
    public float DecayDelay = 0.2f;

    /// <summary>Seconds between each point removal during decay.</summary>
    [DataField]
    public float DecayInterval = 0.03f;

    /// <summary>Optional shader prototype ID applied to the trail geometry.</summary>
    [DataField]
    public string? Shader;
}
