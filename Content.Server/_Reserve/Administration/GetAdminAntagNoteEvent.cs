// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Reserve.Administration;

/// <summary>
/// Raised on a player's attached entity when building admin player info.
/// Lets antag systems add a short note shown to admins, e.g. whose thrall this is.
/// </summary>
[ByRefEvent]
public record struct GetAdminAntagNoteEvent
{
    public string? Note;
}
