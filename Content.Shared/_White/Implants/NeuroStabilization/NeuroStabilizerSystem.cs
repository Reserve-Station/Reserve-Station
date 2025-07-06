// SPDX-FileCopyrightText: 2025 Neverluckz <yanechurka1000.7@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Electrocution;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Components;
using Content.Shared.StatusEffect;

namespace Content.Shared._White.Implants.NeuroStabilization;

public sealed class NeuroStabilizationSystem : EntitySystem
{
    [Dependency] private readonly SharedElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NeuroStabilizationComponent, BeforeStaminaDamageEvent>(BeforeStaminaDamage);
    }

    private void BeforeStaminaDamage(EntityUid uid, NeuroStabilizationComponent component, ref BeforeStaminaDamageEvent args)
    {
        args.Cancelled = true;

        // Сбросить стамину и крит, если вдруг что-то прошло
        if (TryComp<StaminaComponent>(uid, out var stamina))
        {
            stamina.StaminaDamage = 0;
            stamina.Critical = false;
            Dirty(uid, stamina);
        }

        // Снять статус оглушения, если вдруг был наложен
        _status.TryRemoveStatusEffect(uid, "KnockedDown");
        _status.TryRemoveStatusEffect(uid, "Stunned");
        _status.TryRemoveStatusEffect(uid, "Paralyzed");

        // Ваш электрошок, если нужен
        if (!component.Electrocution)
            return;

        var damage = (int) MathF.Round(args.Value * component.DamageModifier);
        _electrocution.TryDoElectrocution(uid, null, damage, component.TimeElectrocution,
            false, 0.5f, 1f, null, true);
    }
}
