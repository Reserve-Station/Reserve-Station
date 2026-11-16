using Content.Shared.Revenant.Components;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Revenant.EntitySystems;

public abstract partial class SharedRevealRevenantOnCollideSystem : EntitySystem
{
    [Dependency] private StatusEffectsSystem _status = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedStunSystem _stun = default!;
    // [Dependency] private readonly IGameTiming _gameTiming = default!; // Reserve edit: Fix warnings

    private readonly ProtoId<StatusEffectPrototype> _corporealStatusId = "Corporeal";
    private readonly ProtoId<StatusEffectPrototype> _stunStatusId = "Stun";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RevealRevenantOnCollideComponent, StartCollideEvent>(OnCollideStart);
    }

    public void OnCollideStart(EntityUid uid, RevealRevenantOnCollideComponent comp, StartCollideEvent args)
    {
        if (!HasComp<RevenantComponent>(args.OtherEntity))
            return;

        if (!string.IsNullOrEmpty(comp.PopupText) && !_status.HasStatusEffect(args.OtherEntity, _corporealStatusId))
            _popup.PopupClient(
                Loc.GetString(comp.PopupText, ("revealer", uid), ("revenant", args.OtherEntity)),
                args.OtherEntity,
                args.OtherEntity
            );

        _status.TryAddStatusEffect<CorporealComponent>(args.OtherEntity, _corporealStatusId, comp.RevealTime, true);

        if (comp.StunTime != null && !_status.HasStatusEffect(args.OtherEntity, _stunStatusId))
            _stun.TryUpdateStunDuration(args.OtherEntity, comp.StunTime.Value);
    }
}
