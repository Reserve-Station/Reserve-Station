// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Gravity;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems;

public abstract class SharedJetpackSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    [Dependency] protected readonly SharedAppearanceSystem Appearance = default!;
    [Dependency] protected readonly SharedContainerSystem Container = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!; // Goobstation
    [Dependency] private readonly InventorySystem _inventory = default!; //Reserve jetpack tweaks fix
    [Dependency] private readonly SharedBuckleSystem _buckle = default!; //Reserve jetpack tweaks fix

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<JetpackComponent, GetItemActionsEvent>(OnJetpackGetAction);
        SubscribeLocalEvent<JetpackComponent, DroppedEvent>(OnJetpackDropped);
        SubscribeLocalEvent<JetpackComponent, ToggleJetpackEvent>(OnJetpackToggle);

        SubscribeLocalEvent<JetpackUserComponent, RefreshWeightlessModifiersEvent>(OnJetpackUserWeightlessMovement);
        SubscribeLocalEvent<JetpackUserComponent, CanWeightlessMoveEvent>(OnJetpackUserCanWeightless);
        SubscribeLocalEvent<JetpackUserComponent, EntParentChangedMessage>(OnJetpackUserEntParentChanged);
        SubscribeLocalEvent<JetpackComponent, EntGotInsertedIntoContainerMessage>(OnJetpackMoved);

        SubscribeLocalEvent<GravityChangedEvent>(OnJetpackUserGravityChanged);
        SubscribeLocalEvent<JetpackComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<JetpackUserComponent, DownedEvent>(OnDowned); // Goobstation

        SubscribeLocalEvent<JetpackUserComponent, MagbootsUpdateStateEvent>(OnMagbootsUpdateState); //Reserve jetpack tweaks fix
    }

    private void OnDowned(Entity<JetpackUserComponent> ent, ref DownedEvent args) // Goobstation
    {
        if (!TryComp<JetpackComponent>(ent.Comp.Jetpack, out var jetpack))
            return;

        SetEnabled(ent.Comp.Jetpack, jetpack, false, ent);

        _popup.PopupClient(Loc.GetString("jetpack-downed"), ent, ent);
    }

    private void OnJetpackUserWeightlessMovement(Entity<JetpackUserComponent> ent, ref RefreshWeightlessModifiersEvent args)
    {
        // Yes this bulldozes the values but primarily for backwards compat atm.
        args.WeightlessAcceleration = ent.Comp.WeightlessAcceleration;
        args.WeightlessModifier = ent.Comp.WeightlessModifier;
        args.WeightlessFriction = ent.Comp.WeightlessFriction;
        args.WeightlessFrictionNoInput = ent.Comp.WeightlessFrictionNoInput;
    }

    private void OnMapInit(EntityUid uid, JetpackComponent component, MapInitEvent args)
    {
        _actionContainer.EnsureAction(uid, ref component.ToggleActionEntity, component.ToggleAction);
        Dirty(uid, component);
    }

    private void OnJetpackUserGravityChanged(ref GravityChangedEvent ev)
    {
        var gridUid = ev.ChangedGridIndex;
        var jetpackQuery = GetEntityQuery<JetpackComponent>();

        var query = EntityQueryEnumerator<JetpackUserComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var user, out var transform))
        {
            if (transform.GridUid == gridUid && ev.HasGravity &&
                jetpackQuery.TryGetComponent(user.Jetpack, out var jetpack))
            {
                _popup.PopupClient(Loc.GetString("jetpack-to-grid"), uid, uid);

                SetEnabled(user.Jetpack, jetpack, false, uid);
            }
        }
    }

    private void OnJetpackDropped(EntityUid uid, JetpackComponent component, DroppedEvent args)
    {
        SetEnabled(uid, component, false, args.User);
    }

    private void OnJetpackMoved(Entity<JetpackComponent> ent, ref EntGotInsertedIntoContainerMessage args)
    {
        if (args.Container.Owner != ent.Comp.JetpackUser)
            SetEnabled(ent, ent.Comp, false, ent.Comp.JetpackUser);
    }

    private void OnJetpackUserCanWeightless(EntityUid uid, JetpackUserComponent component, ref CanWeightlessMoveEvent args)
    {
        args.CanMove = true;
    }

    private void OnJetpackUserEntParentChanged(EntityUid uid, JetpackUserComponent component, ref EntParentChangedMessage args)
    {
        if (TryComp<JetpackComponent>(component.Jetpack, out var jetpack) &&
            (!CanEnableOnGrid(args.Transform.GridUid) || !CheckMagboots(uid))) // Reserve jetpack tweaks
        {
            SetEnabled(component.Jetpack, jetpack, false, uid);

            _popup.PopupClient(Loc.GetString("jetpack-to-grid"), uid, uid);
        }
    }

    private void SetupUser(EntityUid user, EntityUid jetpackUid, JetpackComponent component)
    {
        EnsureComp<JetpackUserComponent>(user, out var userComp);
        component.JetpackUser = user;

        if (TryComp<PhysicsComponent>(user, out var physics))
            _physics.SetBodyStatus(user, physics, BodyStatus.InAir);

        userComp.Jetpack = jetpackUid;
        userComp.WeightlessAcceleration = component.Acceleration;
        userComp.WeightlessModifier = component.WeightlessModifier;
        userComp.WeightlessFriction = component.Friction;
        userComp.WeightlessFrictionNoInput = component.Friction;
        _movementSpeedModifier.RefreshWeightlessModifiers(user);
    }

    private void RemoveUser(EntityUid uid, JetpackComponent component)
    {
        if (!RemComp<JetpackUserComponent>(uid))
            return;

        component.JetpackUser = null;

        if (TryComp<PhysicsComponent>(uid, out var physics))
            _physics.SetBodyStatus(uid, physics, BodyStatus.OnGround);

        _movementSpeedModifier.RefreshWeightlessModifiers(uid);
    }

    private void OnJetpackToggle(EntityUid uid, JetpackComponent component, ToggleJetpackEvent args)
    {
        if (args.Handled)
            return;
        //Reserve jetpack tweaks begin
        if (!TryComp(uid, out TransformComponent? xform))
            return;

        if (!CheckMagboots(args.Performer))
        {
            _popup.PopupClient(Loc.GetString("jetpack-no-magboots-on-grid"), uid, args.Performer);
            return;
        }

        var slotEnumerator = _inventory.GetSlotEnumerator(args.Performer);

        while (slotEnumerator.NextItem(out var item))
        {
            if (TryComp<BuckleComponent>(args.Performer, out var buckleComponent) && buckleComponent.BuckledTo != null)
            {
                _buckle.TryUnbuckle(args.Performer, args.Performer, buckleComponent);
            }
            //To use moonboots with jets
            if (HasComp<AntiGravityClothingComponent>(item))
            {
                SetEnabled(uid, component, !IsEnabled(uid));
                return;
            }
        }
        //if (TryComp(uid, out TransformComponent? xform) && !CanEnableOnGrid(xform.GridUid)) //Reserve jetpack tweaks
        if (!CanEnableOnGrid(xform.GridUid))
        {
            _popup.PopupClient(Loc.GetString("jetpack-no-station"), uid, args.Performer);

            return;
        }
        //Reserve jetpack tweaks end

        if (_standing.IsDown(args.Performer)) // Goobstation
        {
            _popup.PopupClient(Loc.GetString("jetpack-is-down"), uid, args.Performer);

            return;
        }

        SetEnabled(uid, component, !IsEnabled(uid));
    }

    private bool CanEnableOnGrid(EntityUid? gridUid)
    {
        // No and no again! Do not attempt to activate the jetpack on a grid with gravity disabled. You will not be the first or the last to try this.
        // https://discord.com/channels/310555209753690112/310555209753690112/1270067921682694234
        //return gridUid == null ||
        //       (!HasComp<GravityComponent>(gridUid));  //Reserve edit //Don't care :troll:

        //Reserve jetpack tweaks begin
        if (gridUid == null || !TryComp<GravityComponent>(gridUid, out var comp))
            return true;

        return !comp.Enabled;
        //Reserve jetpack tweaks end
    }

    // Reserve jetpack tweaks begin
    /// <summary>
    ///     Checks whether the entity can use jet with magboots
    /// </summary>
    /// <returns>
    ///     true if entity can use jet with magboots
    /// </returns>
    private bool CheckMagboots(EntityUid user)
    {
        var xform = Transform(user);
        if (xform.GridUid is null)
            return true;

        var slotEnumerator = _inventory.GetSlotEnumerator(user);
        while (slotEnumerator.NextItem(out var item))
        {
            if (HasComp<MagbootsComponent>(item) &&
                TryComp<ItemToggleComponent>(item, out var itemToggle) &&
                itemToggle.Activated)
                return false;
        }

        return true;
    }
    // Reserve jetpack tweaks end

    private void OnJetpackGetAction(EntityUid uid, JetpackComponent component, GetItemActionsEvent args)
    {
        args.AddAction(ref component.ToggleActionEntity, component.ToggleAction);
    }

    private bool IsEnabled(EntityUid uid)
    {
        return HasComp<ActiveJetpackComponent>(uid);
    }

    public void SetEnabled(EntityUid uid, JetpackComponent component, bool enabled, EntityUid? user = null)
    {
        if (IsEnabled(uid) == enabled ||
            enabled && !CanEnable(uid, component))
            return;

        if (user == null)
        {
            if (!Container.TryGetContainingContainer((uid, null, null), out var container))
                return;
            user = container.Owner;
        }

        if (enabled)
        {
            SetupUser(user.Value, uid, component);
            EnsureComp<ActiveJetpackComponent>(uid);
        }
        else
        {
            RemoveUser(user.Value, component);
            RemComp<ActiveJetpackComponent>(uid);
        }


        // Goob edit - jetpack state might have changed by the time "enabled" is any relevant
        Appearance.SetData(uid, JetpackVisuals.Enabled, HasComp<ActiveJetpackComponent>(uid));
        Dirty(uid, component);
    }

    public bool IsUserFlying(EntityUid uid)
    {
        return HasComp<JetpackUserComponent>(uid);
    }

    protected virtual bool CanEnable(EntityUid uid, JetpackComponent component)
    {
        return true;
    }
    //Reserve jetpack tweaks begin
    private void OnMagbootsUpdateState(Entity<JetpackUserComponent> ent, ref MagbootsUpdateStateEvent args)
    {
        if (!args.State)
            return;

        if (TryComp<JetpackComponent>(ent.Comp.Jetpack, out var jetpack))
        {
            SetEnabled(ent.Comp.Jetpack, jetpack, false, ent.Owner);
            _popup.PopupClient(Loc.GetString("jetpack-to-grid"), ent.Comp.Jetpack, ent.Owner);
        }
    }
    //Reserve jetpack tweaks end
}

[Serializable, NetSerializable]
public enum JetpackVisuals : byte
{
    Enabled,
    Layer
}
