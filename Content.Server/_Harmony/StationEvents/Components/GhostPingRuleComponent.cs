// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Harmony.StationEvents.Events;
using Robust.Shared.Audio;

namespace Content.Server._Harmony.StationEvents.Components;

[RegisterComponent, Access(typeof(GhostPingRule))]
public sealed partial class GhostPingRuleComponent : Component
{
    /// <summary>
    /// The annoucmene ghosts see on rule added
    /// </summary>
    [DataField]
    public string Announcement = "ghost-ping-annoucement";

    /// <summary>
    /// The sound ghosts here on rule added
    /// </summary>
    [DataField]
    public string PingAudio = "/Audio/Effects/voteding.ogg";
}
