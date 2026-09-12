using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Player;

// Reserve edit: Offer Item system port from WWDP
namespace Content.Shared.OfferItem;

public abstract partial class SharedOfferItemSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<OfferItemComponent, InteractUsingEvent>(SetInReceiveMode);
        SubscribeLocalEvent<OfferItemComponent, AcceptOfferAlertEvent>(OnAcceptOfferAlert);

        InitializeInteractions();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<OfferItemComponent>();
        while (query.MoveNext(out var uid, out var offer))
        {
            if (offer.Target == null)
                continue;

            if (!Exists(offer.Target.Value))
            {
                UnOffer(uid, offer);
                continue;
            }

            if (_transform.InRange(Transform(uid).Coordinates, Transform(offer.Target.Value).Coordinates, offer.MaxOfferDistance))
                continue;

            UnOffer(uid, offer);
        }
    }

    private void SetInReceiveMode(EntityUid uid, OfferItemComponent component, InteractUsingEvent args)
    {
        if (!TryComp<OfferItemComponent>(args.User, out var offerItem))
            return;

        if (args.User == uid || component.IsInReceiveMode || !offerItem.IsInOfferMode ||
            (offerItem.IsInReceiveMode && offerItem.Target != uid))
            return;

        component.IsInReceiveMode = true;
        component.Target = args.User;
        Dirty(uid, component);

        offerItem.Target = uid;
        offerItem.IsInOfferMode = false;
        Dirty(args.User, offerItem);

        if (offerItem.Item == null)
            return;

        _popup.PopupEntity(Loc.GetString("offer-item-try-give",
            ("item", Identity.Entity(offerItem.Item.Value, EntityManager)),
            ("target", Identity.Entity(uid, EntityManager))), component.Target.Value, component.Target.Value);
        _popup.PopupEntity(Loc.GetString("offer-item-try-give-target",
            ("user", Identity.Entity(component.Target.Value, EntityManager)),
            ("item", Identity.Entity(offerItem.Item.Value, EntityManager))), component.Target.Value, uid);

        args.Handled = true;
    }

    private void OnAcceptOfferAlert(EntityUid uid, OfferItemComponent component, AcceptOfferAlertEvent args)
    {
        if (!TryComp<OfferItemComponent>(component.Target, out var offerItem)
            || !TryComp<HandsComponent>(uid, out var hands)
            || offerItem.Hand == null)
            return;

        if (offerItem.Item != null)
        {
            if (!_hands.TryPickup(uid, offerItem.Item.Value, handsComp: hands))
            {
                _popup.PopupClient(Loc.GetString("offer-item-full-hand"), uid, uid);
                return;
            }

            _popup.PopupClient(
                Loc.GetString(
                    "offer-item-give",
                    ("item", Identity.Entity(offerItem.Item.Value, EntityManager)),
                    ("target", Identity.Entity(uid, EntityManager))),
                component.Target.Value,
                component.Target.Value);

            _popup.PopupEntity(
                Loc.GetString(
                    "offer-item-give-other",
                    ("user", Identity.Entity(component.Target.Value, EntityManager)),
                    ("item", Identity.Entity(offerItem.Item.Value, EntityManager)),
                    ("target", Identity.Entity(uid, EntityManager))),
                component.Target.Value,
                Filter.PvsExcept(component.Target.Value, entityManager: EntityManager),
                true);
        }

        offerItem.Item = null;
        UnReceive(uid, component, offerItem);
    }

    protected void UnOffer(EntityUid uid, OfferItemComponent component)
    {
        if (!TryComp(component.Target, out OfferItemComponent? offerItem) || component.Target == null)
            goto ResetSelf;

        if (component.Item != null)
        {
            _popup.PopupEntity(Loc.GetString("offer-item-no-give",
                ("item", Identity.Entity(component.Item.Value, EntityManager)),
                ("target", Identity.Entity(component.Target.Value, EntityManager))), uid, uid);
            _popup.PopupEntity(Loc.GetString("offer-item-no-give-target",
                ("user", Identity.Entity(uid, EntityManager)),
                ("item", Identity.Entity(component.Item.Value, EntityManager))), uid, component.Target.Value);
        }
        else if (offerItem.Item != null)
        {
            _popup.PopupEntity(Loc.GetString("offer-item-no-give",
                ("item", Identity.Entity(offerItem.Item.Value, EntityManager)),
                ("target", Identity.Entity(uid, EntityManager))), component.Target.Value, component.Target.Value);
            _popup.PopupEntity(Loc.GetString("offer-item-no-give-target",
                ("user", Identity.Entity(component.Target.Value, EntityManager)),
                ("item", Identity.Entity(offerItem.Item.Value, EntityManager))), component.Target.Value, uid);
        }

        offerItem.IsInOfferMode = false;
        offerItem.IsInReceiveMode = false;
        offerItem.Hand = null;
        offerItem.Target = null;
        offerItem.Item = null;
        Dirty(component.Target.Value, offerItem);

ResetSelf:
        component.IsInOfferMode = false;
        component.IsInReceiveMode = false;
        component.Hand = null;
        component.Target = null;
        component.Item = null;
        Dirty(uid, component);
    }

    protected void UnReceive(EntityUid uid, OfferItemComponent? component = null, OfferItemComponent? offerItem = null)
    {
        if (component == null && !TryComp(uid, out component))
            return;

        if (offerItem == null && !TryComp(component.Target, out offerItem))
            return;

        if (component.Target == null)
            return;

        if (offerItem.Item != null)
        {
            _popup.PopupEntity(Loc.GetString("offer-item-no-give",
                ("item", Identity.Entity(offerItem.Item.Value, EntityManager)),
                ("target", Identity.Entity(uid, EntityManager))), component.Target.Value, component.Target.Value);
            _popup.PopupEntity(Loc.GetString("offer-item-no-give-target",
                ("user", Identity.Entity(component.Target.Value, EntityManager)),
                ("item", Identity.Entity(offerItem.Item.Value, EntityManager))), component.Target.Value, uid);
        }

        if (!offerItem.IsInReceiveMode)
        {
            offerItem.Target = null;
            component.Target = null;
        }

        offerItem.Item = null;
        offerItem.Hand = null;
        component.IsInReceiveMode = false;

        Dirty(uid, component);
    }

    protected bool IsInOfferMode(EntityUid? entity, OfferItemComponent? component = null)
    {
        return entity != null && Resolve(entity.Value, ref component, false) && component.IsInOfferMode;
    }
}

