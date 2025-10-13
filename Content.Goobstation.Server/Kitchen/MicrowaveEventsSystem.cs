// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 X <70487315+XWasHere@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Kitchen.Components;
using Robust.Shared.Containers;

namespace Content.Goobstation.Server.Kitchen;

/// <summary>
/// Prevents automation taking items out of an active microwave.
/// Only exists because microwave supercode only prevents it in interaction, not attempt events.
/// </summary>
public sealed class MicrowaveEventsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActiveMicrowaveComponent, ContainerIsRemovingAttemptEvent>(OnRemoveAttempt);
    }

    private void OnRemoveAttempt(Entity<ActiveMicrowaveComponent> ent, ref ContainerIsRemovingAttemptEvent args)
    {
        if (ent.Comp.CookTimeRemaining > 0)
            args.Cancel();
    }
}
