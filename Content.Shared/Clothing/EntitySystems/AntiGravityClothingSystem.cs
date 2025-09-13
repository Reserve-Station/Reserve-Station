// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Clothing.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class AntiGravityClothingSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly SharedJetpackSystem _jetpackSystem = default!; //Reserve jetpack tweaks
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!; //Reserve jetpack tweaks
    public override void Initialize()
    {
        SubscribeLocalEvent<AntiGravityClothingComponent, InventoryRelayedEvent<IsWeightlessEvent>>(OnIsWeightless);
        SubscribeLocalEvent<AntiGravityClothingComponent, ClothingGotUnequippedEvent>(OnUnequipped); //Reserve jetpack tweaks
    }

    private void OnIsWeightless(Entity<AntiGravityClothingComponent> ent, ref InventoryRelayedEvent<IsWeightlessEvent> args)
    {
        if (args.Args.Handled)
            return;

        args.Args.Handled = true;
        args.Args.IsWeightless = true;
    }
    //Reserve jetpack tweaks begin
    private void OnUnequipped(Entity<AntiGravityClothingComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        if (TryComp<JetpackUserComponent>(args.Wearer, out var jetpackUserComp) &&
            TryComp<JetpackComponent>(jetpackUserComp.Jetpack, out var jetpack))
        {
            _jetpackSystem.SetEnabled(jetpackUserComp.Jetpack, jetpack, false, args.Wearer);
            _popupSystem.PopupClient(Loc.GetString("jetpack-to-grid"), jetpackUserComp.Jetpack, args.Wearer);
        }
    }
    //Reserve jetpack tweaks end
}
