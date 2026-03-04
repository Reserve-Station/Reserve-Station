// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 NazrinNya <137837419+NazrinNya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 nazrin <tikufaev@outlook.com>
// SPDX-FileCopyrightText: 2025 sa1nt7331 <202271576+sa1nt7331@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage.Systems;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Throwing;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using System.Numerics;
using Content.Shared._White.Standing;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Components;

namespace Content.Shared._White.Grab;

public sealed class GrabThrownSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly INetManager _netMan = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GrabThrownComponent, StartCollideEvent>(HandleCollide);
        SubscribeLocalEvent<GrabThrownComponent, StopThrowEvent>(OnStopThrow);
    }

    private void HandleCollide(Entity<GrabThrownComponent> ent, ref StartCollideEvent args)
    {
        if (_netMan.IsClient) // To avoid effect spam
            return;

        if (!HasComp<ThrownItemComponent>(ent))
        {
            RemComp<GrabThrownComponent>(ent);
            return;
        }

        if (ent.Comp.IgnoreEntity.Contains(args.OtherEntity))
            return;

        if (!HasComp<DamageableComponent>(ent))
            RemComp<GrabThrownComponent>(ent);

        if(!TryComp<PhysicsComponent>(ent, out var physicsComponent))
            return;

        ent.Comp.IgnoreEntity.Add(args.OtherEntity);

        var velocitySquared = args.OurBody.LinearVelocity.LengthSquared();
        var mass = physicsComponent.Mass;
        var kineticEnergy = 0.5f * mass * velocitySquared;
        var kineticEnergyDamage = new DamageSpecifier();
        kineticEnergyDamage.DamageDict.Add("Blunt", 1);
        var modNumber = Math.Floor(kineticEnergy / 100);
        kineticEnergyDamage *= Math.Floor(modNumber / 3);
        _damageable.TryChangeDamage(args.OtherEntity, kineticEnergyDamage);
        _stamina.TakeStaminaDamage(ent, (float) Math.Floor(modNumber / 2));

        _stun.TryCrawling(args.OtherEntity);

        _color.RaiseEffect(Color.Red, new List<EntityUid>() { ent }, Filter.Pvs(ent, entityManager: EntityManager));
    }

    private void OnStopThrow(EntityUid uid, GrabThrownComponent comp, StopThrowEvent args)
    {
        if (_netMan.IsClient) //Reserve edit
            return;
        if (comp.DamageOnCollide != null)
            _damageable.TryChangeDamage(uid, comp.DamageOnCollide);

        RemComp<GrabThrownComponent>(uid); //Reserve edit
    }

    /// <summary>
    /// Throwing entity to the direction and ensures GrabThrownComponent with params
    /// </summary>
    /// <param name="uid">Entity to throw</param>
    /// <param name="thrower">Entity that throws</param>
    /// <param name="vector">Direction</param>
    /// <param name="grabThrownSpeed">How fast you fly when thrown</param>
    /// <param name="staminaDamage">Stamina damage on collide</param>
    /// <param name="damageToUid">Damage to entity on collide</param>
    public void Throw(
        EntityUid uid,
        EntityUid thrower,
        Vector2 vector,
        float grabThrownSpeed,
        DamageSpecifier? damageToUid = null,
        bool behavior = false) // Goob edit
    {
        var comp = EnsureComp<GrabThrownComponent>(uid);
        comp.IgnoreEntity.Add(thrower);
        comp.DamageOnCollide = damageToUid;

        _stun.TryCrawling(uid);
        _throwing.TryThrow(uid, vector, grabThrownSpeed, animated: false);
    }
}
