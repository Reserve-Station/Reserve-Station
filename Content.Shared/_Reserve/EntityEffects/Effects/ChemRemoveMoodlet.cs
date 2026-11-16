// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Mood;
using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.EntityEffects.Effects;

/// <summary>
///     Removes a moodlet from an entity if present.
/// </summary>
[UsedImplicitly]
public sealed partial class ChemRemoveMoodlet : EntityEffectBase<ChemRemoveMoodlet>
{
    /// <summary>
    ///     The mood prototype to be removed from the entity.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<MoodEffectPrototype> MoodPrototype;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        var moodPrototype = prototype.Index<MoodEffectPrototype>(MoodPrototype.Id);
        return Loc.GetString("reagent-effect-guidebook-remove-moodlet",
            ("name", moodPrototype.Description()));
    }
}
