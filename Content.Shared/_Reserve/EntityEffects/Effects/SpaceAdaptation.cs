// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization;
using Content.Shared.Body.Components;

namespace Content.Shared._Reserve.EntityEffects.Effects;

public sealed partial class SpaceAdaptation : EntityEffectBase<SpaceAdaptation>
{
    [DataField("spaceHeartProto")]
    public string SpaceHeartProto = "OrganSpaceAnimalHeart";

    [DataField("spaceLungsProto")]
    public string SpaceLungsProto = "OrganSpaceAnimalLungs";

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-space-adaptation");
    }
}
