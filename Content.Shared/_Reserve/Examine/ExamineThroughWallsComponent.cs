// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Reserve.Examine;

/// <summary>
/// Lets the examiner examine entities behind walls, as long as they are in examine range.
/// Meant for entities that can see through walls (e.g. Umbrae vampire at full power).
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ExamineThroughWallsComponent : Component;
