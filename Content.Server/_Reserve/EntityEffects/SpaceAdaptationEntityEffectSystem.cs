// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Reserve.EntityEffects.Effects;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.EntityEffects;
using Robust.Shared.Containers;

namespace Content.Server._Reserve.EntityEffects;

public sealed partial class SpaceAdaptationEntityEffectSystem : EntityEffectSystem<BodyComponent, SpaceAdaptation>
{
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly SharedContainerSystem _containers = default!;

    protected override void Effect(Entity<BodyComponent> entity, ref EntityEffectEvent<SpaceAdaptation> args)
    {
        var replacements = new List<(EntityUid Organ, OrganComponent Comp, string Proto)>();

        foreach (var (organUid, organComp) in _body.GetBodyOrgans(entity.Owner, entity.Comp))
        {
            if (HasComp<HeartComponent>(organUid))
                replacements.Add((organUid, organComp, args.Effect.SpaceHeartProto));
            else if (HasComp<LungComponent>(organUid))
                replacements.Add((organUid, organComp, args.Effect.SpaceLungsProto));
        }

        foreach (var (organUid, organComp, proto) in replacements)
            TryReplaceOrgan(entity, organUid, organComp, proto);
    }

    private void TryReplaceOrgan(EntityUid body, EntityUid organUid, OrganComponent organComp, string proto)
    {
        if (MetaData(organUid).EntityPrototype?.ID == proto)
            return;

        if (string.IsNullOrEmpty(organComp.SlotId))
            return;

        if (!_containers.TryGetContainingContainer(organUid, out var container))
            return;

        var partUid = container.Owner;
        var slotId = organComp.SlotId;
        var newOrgan = Spawn(proto, Transform(body).Coordinates);

        _body.RemoveOrgan(organUid);
        QueueDel(organUid);

        if (!_body.InsertOrgan(partUid, newOrgan, slotId))
            QueueDel(newOrgan);
    }
}
