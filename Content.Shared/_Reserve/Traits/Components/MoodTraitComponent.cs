// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Mood;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.Traits.Components;

[RegisterComponent]
public sealed partial class MoodTraitComponent : Component
{
    [DataField(required: true)]
    public List<ProtoId<MoodEffectPrototype>> MoodEffects = new();
}
