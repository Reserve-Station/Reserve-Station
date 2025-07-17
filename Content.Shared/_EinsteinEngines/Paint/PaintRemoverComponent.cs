// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared._EinsteinEngines.Paint;

///  Removes paint from an entity that was painted with spray paint
[RegisterComponent, NetworkedComponent]
[Access(typeof(PaintRemoverSystem))]
public sealed partial class PaintRemoverComponent : Component
{
    /// Sound played when target is cleaned
    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/Fluids/watersplash.ogg");

    [DataField]
    public float CleanDelay = 2f;
}
