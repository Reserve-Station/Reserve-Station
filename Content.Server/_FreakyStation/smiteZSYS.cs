// SPDX-FileCopyrightText: 2025 ANIvarmin <danilter2021@gmail.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Robust.Shared.Prototypes;
using Robust.Shared.Player;
using Robust.Server.Player;
using Content.Shared.Interaction.Components;
using Content.Shared._White.CustomGhostSystem;
using Content.Shared._FreakyStation;

namespace Content.Server._FreakyStation;

public sealed class SmiteZSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActorComponent, PlayerAttachedEvent>(OnPlayer);

    }

    private void OnPlayer(EntityUid uid, ActorComponent component, PlayerAttachedEvent args)
    {
        if (!_playerManager.TryGetSessionByEntity(uid, out var session))
            return;

        TrySetSmite(uid, session.Name);


    }

    public void TrySetSmite(EntityUid uid, string ckey)
    {


        var prototypes = _prototypeManager.EnumeratePrototypes<SmiteZPrototype>();

        foreach (var SmiteZPrototype in prototypes)
        {
            if (!string.Equals(SmiteZPrototype.Ckey, ckey, StringComparison.CurrentCultureIgnoreCase))
                continue;

            if (ckey != null)
            {
                if (TryComp<InventoryComponent>(uid, out var inventory))
                {
                    var ears = Spawn("ClothingHeadHatCatEars", Transform(uid).Coordinates);
                    EnsureComp<UnremoveableComponent>(ears);
                    _inventory.TryUnequip(uid, "head", true, true, false, inventory);
                    _inventory.TryEquip(uid, ears, "head", true, true, false, inventory);
                }
            }


        }
    }

}
