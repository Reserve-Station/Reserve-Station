// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.Mood;
using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.EntityEffects.Effects;

/// <summary>
///     Removes all non-categorized moodlets from an entity(anything not "Static" like hunger & thirst).
/// </summary>
[UsedImplicitly]
public sealed partial class ChemPurgeMoodlets : EntityEffectBase<ChemPurgeMoodlets>
{
    [DataField]
    public bool RemovePermanentMoodlets;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-purge-moodlets");
}
