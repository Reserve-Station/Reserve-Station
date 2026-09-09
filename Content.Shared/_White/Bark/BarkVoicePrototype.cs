// SPDX-FileCopyrightText: 2025 Cinkafox <70429757+Cinkafox@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Customization.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._White.Bark;

[Prototype]
public sealed partial class BarkVoicePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public SoundSpecifier BarkSound { get; set; } = default!;

    [DataField]
    public BarkClampData ClampData { get; set; } = new();
}

[Prototype]
public sealed partial class BarkListPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public Dictionary<ProtoId<BarkVoicePrototype>, List<CharacterRequirement>> VoiceList { get; set; } = [];
}
