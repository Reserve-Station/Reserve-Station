// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
public sealed partial class SliceableFoodComponent : Component
{
    [DataField]
    public EntProtoId? Slice;

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Items/Culinary/chop.ogg");

    [DataField("count")]
    public ushort TotalCount = 5;

    [DataField]
    public float SliceTime = 1f;

    [DataField]
    public float SpawnOffset = 0.5f;
}
