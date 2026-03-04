// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Reserve.Inventory.UI;
using Content.Server._Reserve.LenaApi;
using Content.Server.EUI;
using Content.Shared._Reserve.Ghost;
using Content.Shared.Actions;
using Content.Shared.Ghost;
using Robust.Shared.Player;

namespace Content.Server._Reserve.Ghost;

public sealed class GhostInventorySystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly LenaApiManager _lenaApi = default!;
    [Dependency] private readonly EuiManager _euiManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GhostComponent, PlayerAttachedEvent>(OnGhostPlayerAttached);
        SubscribeLocalEvent<GhostComponent, GhostInventoryActionEvent>(OnGhostInventoryAction);
    }

    private void OnGhostPlayerAttached(EntityUid uid, GhostComponent _, PlayerAttachedEvent args)
    {
        if (!_lenaApi.IsIntegrationEnabled)
            return;

        EntityUid? actionEntity = null;
        _actions.AddAction(uid, ref actionEntity, "ActionGhostInventory");
    }

    private void OnGhostInventoryAction(EntityUid uid, GhostComponent _, GhostInventoryActionEvent args)
    {
        if (!TryComp<ActorComponent>(uid, out var actor))
            return;

        _euiManager.OpenEui(new InventoryEui(), actor.PlayerSession);
        args.Handled = true;
    }
}
