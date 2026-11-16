using Content.Goobstation.Common.Weapons.NoWieldNeeded;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.Revenant.Components;
using Content.Shared.CombatMode;
using Content.Shared.Construction.Components;
using Content.Shared.Cuffs.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Revenant.Components;
using Content.Shared.Throwing;
using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Revenant.EntitySystems;

// Reserve edit - rewritten: normal items now hover and throw themselves instead of becoming walking mobs.
// Guns, handcuffs and grenades keep the old NPC behaviour.
/// <summary>
/// Animated items hover in place and hurl themselves at the closest living being in range.
/// Guns, handcuffs and grenades instead get an NPC brain: guns shoot, handcuffs cuff and grenades jump into pockets and arm themselves.
/// The hover animation and purple glow are handled client-side.
/// </summary>
public sealed partial class RevenantAnimatedSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly NpcFactionSystem _faction = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _moveSpeed = default!;
    [Dependency] private readonly MobThresholdSystem _thresholds = default!;

    private static readonly ProtoId<NpcFactionPrototype> HostileFaction = "SimpleHostile";
    private static readonly ProtoId<DamageTypePrototype> BluntDamage = "Blunt";

    private readonly HashSet<Entity<MobStateComponent>> _targets = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RevenantAnimatedComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RevenantAnimatedComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<RevenantAnimatedComponent, ThrowDoHitEvent>(OnThrowHit);
        SubscribeLocalEvent<RevenantAnimatedComponent, GettingPickedUpAttemptEvent>(OnPickupAttempt);
        SubscribeLocalEvent<RevenantAnimatedComponent, MobStateChangedEvent>(OnMobStateChange);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;
        var enumerator = EntityQueryEnumerator<RevenantAnimatedComponent>();

        while (enumerator.MoveNext(out var uid, out var animate))
        {
            if (animate.EndTime <= curTime)
            {
                InanimateTarget(uid, animate);
                continue;
            }

            if (animate.HasBrain
                || animate.NextThrow > curTime
                || HasComp<ThrownItemComponent>(uid)
                || _container.IsEntityInContainer(uid))
                continue;

            if (TryFindTarget((uid, animate)) is not { } target)
                continue;

            animate.NextThrow = curTime + animate.ThrowCooldown;
            _throwing.TryThrow(uid, Transform(target).Coordinates, animate.ThrowSpeed, recoil: false);
        }
    }

    /// <summary>
    /// Finds the closest living being in range that the item can see.
    /// </summary>
    private EntityUid? TryFindTarget(Entity<RevenantAnimatedComponent> ent)
    {
        var coords = _transform.GetMapCoordinates(ent);
        _targets.Clear();
        _lookup.GetEntitiesInRange(coords, ent.Comp.DetectRange, _targets);

        EntityUid? closest = null;
        var closestDist = float.MaxValue;

        foreach (var target in _targets)
        {
            if (HasComp<RevenantComponent>(target)
                || HasComp<RevenantAnimatedComponent>(target)
                || _mobState.IsDead(target, target.Comp)
                || !_interaction.InRangeUnobstructed(ent.Owner, target.Owner, ent.Comp.DetectRange))
                continue;

            var dist = (_transform.GetMapCoordinates(target).Position - coords.Position).LengthSquared();
            if (dist >= closestDist)
                continue;

            closest = target;
            closestDist = dist;
        }

        return closest;
    }

    private void OnComponentStartup(Entity<RevenantAnimatedComponent> ent, ref ComponentStartup _)
    {
        // Give people a moment to notice the item before it starts flying.
        ent.Comp.NextThrow = _gameTiming.CurTime + ent.Comp.ThrowCooldown;

        _popup.PopupEntity(Loc.GetString("revenant-animate-item-animate", ("target", ent.Owner)), ent.Owner, Filter.Pvs(ent.Owner), true);

        if (NeedsBrain(ent))
            GiveBrain(ent);
    }

    /// <summary>
    /// Guns, handcuffs and grenades have to get close to people and use themselves, so they get an NPC instead of being thrown.
    /// </summary>
    public bool NeedsBrain(EntityUid uid)
    {
        return HasComp<GunComponent>(uid) || HasComp<HandcuffComponent>(uid) || HasComp<TriggerOnUseComponent>(uid);
    }

    private void GiveBrain(Entity<RevenantAnimatedComponent> ent)
    {
        ent.Comp.HasBrain = true;

        // Melee for guns that ran out of ammo
        if (EnsureHelper<MeleeWeaponComponent>(ent, out var melee))
            melee.Damage = new DamageSpecifier(_prototypeManager.Index(BluntDamage), 5);

        EnsureHelper<InputMoverComponent>(ent);
        EnsureHelper<MovementSpeedModifierComponent>(ent, out var moveSpeed);
        _moveSpeed.ChangeBaseSpeed(ent,
            ent.Comp.Revenant?.Comp.AnimateWalkSpeed ?? RevenantComponent.DefaultAnimateWalkSpeed,
            ent.Comp.Revenant?.Comp.AnimateSprintSpeed ?? RevenantComponent.DefaultAnimateSprintSpeed,
            MovementSpeedModifierComponent.DefaultAcceleration,
            moveSpeed);

        EnsureHelper<NpcFactionMemberComponent>(ent, out var factions);
        _faction.ClearFactions((ent, factions));
        _faction.AddFaction((ent, factions), HostileFaction);

        // For things like handcuffs
        EnsureHelper<DoAfterComponent>(ent);

        EnsureHelper<HTNComponent>(ent, out var htn);
        if (HasComp<GunComponent>(ent))
        {
            // Goals: Magdump into any nearby creatures, and melee hit them if empty
            if (TryComp<ChamberMagazineAmmoProviderComponent>(ent, out var bolt))
                _gun.SetBoltClosed(ent, bolt, true);
            // Two-handed guns refuse to shoot unless wielded, and a gun has no hands to wield itself with
            EnsureHelper<NoWieldNeededComponent>(ent);
            htn.RootTask = new HTNCompoundTask { Task = "SimpleRangedHostileCompound" };
        }
        else if (HasComp<HandcuffComponent>(ent))
            // Goals: Jump into any creature's pockets/hands and cuff them
            htn.RootTask = new HTNCompoundTask { Task = "AnimatedHandcuffsCompound" };
        else
            // Goals: Jump into any creature's pockets/hands and activate self
            htn.RootTask = new HTNCompoundTask { Task = "AnimatedGrenadeCompound" };

        htn.Blackboard.SetValue(NPCBlackboard.Owner, ent.Owner);

        EnsureHelper<DamageableComponent>(ent);
        EnsureHelper<MobStateComponent>(ent);
        EnsureHelper<MobThresholdsComponent>(ent, out var thresholds);
        _thresholds.SetMobStateThreshold(ent, 30, MobState.Dead, thresholds);

        EnsureHelper<CombatModeComponent>(ent);
    }

    // Returns true if a new component was added to the target.
    private bool EnsureHelper<T>(Entity<RevenantAnimatedComponent> target, out T comp) where T : Component, new()
    {
        if (TryComp<T>(target, out var existing))
        {
            comp = existing;
            return false;
        }

        comp = AddComp<T>(target);
        target.Comp.AddedComponents.Add(comp);
        return true;
    }

    private bool EnsureHelper<T>(Entity<RevenantAnimatedComponent> target) where T : Component, new()
    {
        return EnsureHelper<T>(target, out _);
    }

    private void OnComponentShutdown(Entity<RevenantAnimatedComponent> ent, ref ComponentShutdown _)
    {
        if (TerminatingOrDeleted(ent))
            return;

        foreach (var comp in ent.Comp.AddedComponents)
        {
            if (comp.Deleted)
                continue;

            RemCompDeferred(ent, comp);
        }

        _popup.PopupEntity(Loc.GetString("revenant-animate-item-inanimate", ("target", ent)), ent, Filter.Pvs(ent), true);
    }

    /// <summary>
    /// Deals extra damage when the item hits someone, since most items barely hurt when thrown.
    /// </summary>
    private void OnThrowHit(Entity<RevenantAnimatedComponent> ent, ref ThrowDoHitEvent args)
    {
        // Only throws the item made by itself, not ones made by players.
        if (args.Component.Thrower != null || !HasComp<MobStateComponent>(args.Target))
            return;

        var damage = new DamageSpecifier(_prototypeManager.Index(ent.Comp.HitDamageType), ent.Comp.HitDamage);
        _damageable.TryChangeDamage(args.Target, damage, origin: ent.Comp.Revenant?.Owner);
    }

    /// <summary>
    /// Animated items can't be picked up, otherwise they would be trivial to deal with.
    /// </summary>
    private void OnPickupAttempt(Entity<RevenantAnimatedComponent> ent, ref GettingPickedUpAttemptEvent args)
    {
        // Handcuffs and grenades jump into hands and pockets on their own.
        if (ent.Comp.HasBrain && !HasComp<GunComponent>(ent))
            return;

        args.Cancel();
    }

    private void OnMobStateChange(Entity<RevenantAnimatedComponent> ent, ref MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead)
            InanimateTarget(ent, ent.Comp);
    }

    public bool CanAnimateObject(EntityUid target)
    {
        return !(HasComp<AnchorableComponent>(target) || HasComp<MindContainerComponent>(target)
            || HasComp<HTNComponent>(target) || HasComp<RevenantAnimatedComponent>(target)
            || _container.IsEntityInContainer(target));
    }

    public bool TryAnimateObject(EntityUid target, TimeSpan? duration = null, Entity<RevenantComponent>? revenant = null)
    {
        if (!CanAnimateObject(target))
            return false;

        // Fields are set before adding so startup already knows the revenant.
        var animate = new RevenantAnimatedComponent { Revenant = revenant };
        if (duration != null)
            animate.EndTime = _gameTiming.CurTime + duration.Value;
        else if (revenant != null)
            animate.EndTime = _gameTiming.CurTime + revenant.Value.Comp.AnimateTime;

        AddComp(target, animate);

        return true;
    }

    public void InanimateTarget(EntityUid target, RevenantAnimatedComponent? comp = null)
    {
        if (!target.Valid || !Resolve(target, ref comp))
            return;

        RemComp<RevenantAnimatedComponent>(target);
    }
}
