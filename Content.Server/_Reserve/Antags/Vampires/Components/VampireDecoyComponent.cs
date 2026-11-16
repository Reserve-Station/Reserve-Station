// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Reserve.Antags.Vampires.Components;

/// <summary>
/// Handles the simple runtime behaviour for vampire decoy entities.
/// </summary>
[RegisterComponent]
public sealed partial class VampireDecoyComponent : Component
{
    /// <summary>
    /// Ensures the flash/explosion only happens once.
    /// </summary>
    public bool Detonated;
}
