// SPDX-FileCopyrightText: 2025 Hero010h <163765999+Hero010h@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.EntityEffects.Effects;

[Serializable, NetSerializable]
public sealed partial class SpaceAdaptation : EntityEffect
{
    [DataField("spaceHeartProto")]
    public string SpaceHeartProto = "OrganSpaceAnimalHeart";

    [DataField("spaceLungsProto")]
    public string SpaceLungsProto = "OrganSpaceAnimalLungs";

    public override void Effect(EntityEffectBaseArgs args)
    {
        var ev = new ExecuteEntityEffectEvent<SpaceAdaptation>(this, args);
        args.EntityManager.EventBus.RaiseEvent(EventSource.Local, ref ev);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-space-adaptation");
    }
}
