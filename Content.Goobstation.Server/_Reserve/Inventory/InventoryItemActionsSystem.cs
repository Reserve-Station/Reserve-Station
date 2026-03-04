// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.Changeling.GameTicking.Rules;
using Content.Goobstation.Server.Devil.GameTicking.Rules;
using Content.Goobstation.Server.Shadowling.Rules;
using Content.Server._DV.CosmicCult.Components;
using Content.Server._Reserve.Inventory.UI;
using Content.Server._Reserve.LenaApi;
using Content.Server.Antag;
using Content.Server.EUI;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Popups;
using Content.Shared._Reserve.TokenCvars;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Player;

namespace Content.Goobstation.Server._Reserve.Inventory;

public sealed class InventoryItemActionsSystem : EntitySystem
{
    [Dependency] private readonly LenaApiManager _lenaApi = default!;
    [Dependency] private readonly EuiManager _euiManager = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly AntagSelectionSystem _antagSelection = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);

        _lenaApi.RegisterItemAction("low_tier_token", (session, item) => OpenAntagSelection(session, item.ItemId));
        _lenaApi.RegisterItemIcon("low_tier_token",
            "/Textures/Objects/Specific/Syndicate/telecrystal.rsi/telecrystal.png");
        _lenaApi.RegisterAntagRule("low_tier_token",
            "Thief",
            "Вор",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ThiefRuleComponent>(session, "Thief"));


        _lenaApi.RegisterItemAction("ghost_tier_token", (session, item) => OpenAntagSelection(session, item.ItemId));
        _lenaApi.RegisterItemIcon("ghost_tier_token",
            "/Textures/Effects/crayondecals.rsi/ghost.png");
        _lenaApi.RegisterAntagRule("ghost_tier_token", "SkeletonMidround", "Скелет из шкафа", forAlive: false);
        _lenaApi.RegisterAntagRule("ghost_tier_token", "LoneAbductorSpawn", "Одинокий абдуктор", forAlive: false);
        _lenaApi.RegisterAntagRule("ghost_tier_token", "GreyTideAntagMidround", "Грейтайд", forAlive: false);
        _lenaApi.RegisterAntagRule("ghost_tier_token", "MimeAssassinMidround", "Мим-ассасин", forAlive: false);
        _lenaApi.RegisterAntagRule("ghost_tier_token", "TunnelClownMidround", "Клоун-гоблин", forAlive: false);


        _lenaApi.RegisterItemAction("mid_tier_token", (session, item) => OpenAntagSelection(session, item.ItemId));
        _lenaApi.RegisterItemIcon("mid_tier_token",
            "/Textures/Objects/Weapons/Melee/e_sword.rsi/icon.png");
        _lenaApi.RegisterAntagRule("mid_tier_token",
            "Thief",
            "Вор",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ThiefRuleComponent>(session, "Thief"));
        _lenaApi.RegisterAntagRule("mid_tier_token", "SkeletonMidround", "Скелет из шкафа", forAlive: false);
        _lenaApi.RegisterAntagRule("mid_tier_token", "LoneAbductorSpawn", "Одинокий абдуктор", forAlive: false);
        _lenaApi.RegisterAntagRule("mid_tier_token",
            "Traitor",
            "Предатель",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ChangelingRuleComponent>(session, "Traitor"));
        _lenaApi.RegisterAntagRule("mid_tier_token",
            "Changeling",
            "Генокрад",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ChangelingRuleComponent>(session, "Changeling"));
        _lenaApi.RegisterAntagRule("mid_tier_token",
            "Devil",
            "Дьявол",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<DevilRuleComponent>(session, "Devil"));
        _lenaApi.RegisterAntagRule("mid_tier_token", "NinjaSpawn", "Ниндзя", forAlive: false);
        _lenaApi.RegisterAntagRule("mid_tier_token", "GreyTideAntagMidround", "Грейтайд", forAlive: false);
        _lenaApi.RegisterAntagRule("mid_tier_token", "MimeAssassinMidround", "Мим-ассасин", forAlive: false);
        _lenaApi.RegisterAntagRule("mid_tier_token", "TunnelClownMidround", "Клоун-гоблин", forAlive: false);


        _lenaApi.RegisterItemAction("high_tier_token", (session, item) => OpenAntagSelection(session, item.ItemId));
        _lenaApi.RegisterItemIcon("high_tier_token", "/Textures/Clothing/Mask/gassyndicate.rsi/icon.png");
        _lenaApi.RegisterAntagRule("high_tier_token", "SkeletonMidround", "Скелет из шкафа", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "LoneAbductorSpawn", "Одинокий абдуктор", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "NinjaSpawn", "Ниндзя", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "LoneOpsSpawn", "Ядерный оперативник", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "Wizard", "Маг", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "BlobMidround", "Блоб", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "GreyTideAntagMidround", "Грейтайд", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "MimeAssassinMidround", "Мим-ассасин", forAlive: false);
        _lenaApi.RegisterAntagRule("high_tier_token", "TunnelClownMidround", "Клоун-гоблин", forAlive: false);

        _lenaApi.RegisterAntagRule("high_tier_token",
            "Thief",
            "Вор",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ThiefRuleComponent>(session, "Thief"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Traitor",
            "Предатель",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ChangelingRuleComponent>(session, "Traitor"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Changeling",
            "Генокрад",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ChangelingRuleComponent>(session, "Changeling"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Devil",
            "Дьявол",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<DevilRuleComponent>(session, "Devil"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Zombie",
            "Нулевой зараженный",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<ZombieRuleComponent>(session, "Zombie"));
        // наверное перенесу позже в новый токен
        // _lenaApi.RegisterAntagRule("high_tier_token",
        //     "CosmicCult",
        //     "Космический культист",
        //     forAlive: true,
        //     forAliveAction: session => _antagSelection.ForceMakeAntag<CosmicCultRuleComponent>(session, "CosmicCult"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Heretic",
            "Еретик",
            forAlive: true,
            forAliveAction: session => _antagSelection.ForceMakeAntag<HereticRuleComponent>(session, "Heretic"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Revolutionary",
            "Глава революции",
            forAlive: true,
            forAliveAction: session =>
                _antagSelection.ForceMakeAntag<RevolutionaryRuleComponent>(session, "Revolutionary"));
        _lenaApi.RegisterAntagRule("high_tier_token",
            "Shadowling",
            "Тенеморф",
            forAlive: true,
            forAliveAction: session =>
                _antagSelection.ForceMakeAntag<ShadowlingRuleComponent>(session, "Shadowling"));

        _lenaApi.RegisterItemAction("admin_abuse_tier_token",
            (session, item) => OpenCosmeticSelection(session, item.ItemId));
        _lenaApi.RegisterItemIcon("admin_abuse_tier_token",
            "/Textures/Clothing/OuterClothing/Coats/syndicate/coatsyndiecap.rsi/icon.png");

        List<string> adminAbuseItems =
        [
            "ClothingNeckCloakGay",
        ];
        foreach (var protoId in adminAbuseItems)
        {
            _lenaApi.RegisterCosmeticItem("admin_abuse_tier_token", protoId);
        }

        _lenaApi.RegisterTokenConditions("ghost_tier_token",
            new LenaApiManager.TokenConditions(
                TokenCvars.GhostTierTokenMinAlive,
                TokenCvars.GhostTierTokenMaxAntagAlive,
                TokenCvars.GhostTierTokenChance,
                TokenCvars.GhostTierTokenMinSecAlive,
                BlockingRules:
                [
                    "Revolutionary", "Heretic", "CosmicCult", "Zombie", "LoneOpsSpawn", "NinjaSpawn", "Honkops",
                    "NukeopsRule", "PiratesRule", "LoneAbductorSpawn", "DuoAbductorSpawn", "BlobRule",
                ]
            ));

        _lenaApi.RegisterTokenConditions("low_tier_token",
            new LenaApiManager.TokenConditions(
                TokenCvars.LowTierTokenMinAlive,
                TokenCvars.LowTierTokenMaxAntagAlive,
                TokenCvars.LowTierTokenChance,
                TokenCvars.LowTierTokenMinSecAlive,
                BlockingRules:
                [
                    "Revolutionary", "Heretic", "CosmicCult", "Zombie", "LoneOpsSpawn", "NinjaSpawn", "Honkops",
                    "NukeopsRule", "PiratesRule", "LoneAbductorSpawn", "DuoAbductorSpawn", "BlobRule",
                ]
            ));

        _lenaApi.RegisterTokenConditions("mid_tier_token",
            new LenaApiManager.TokenConditions(
                TokenCvars.MidTierTokenMinAlive,
                TokenCvars.MidTierTokenMaxAntagAlive,
                TokenCvars.MidTierTokenChance,
                TokenCvars.MidTierTokenMinSecAlive,
                BlockingRules:
                [
                    "Revolutionary", "Heretic", "CosmicCult", "Zombie", "LoneOpsSpawn", "NinjaSpawn", "Honkops",
                    "NukeopsRule", "PiratesRule", "LoneAbductorSpawn", "DuoAbductorSpawn", "BlobRule",
                ]
            ));

        _lenaApi.RegisterTokenConditions("high_tier_token",
            new LenaApiManager.TokenConditions(
                TokenCvars.HighTierTokenMinAlive,
                TokenCvars.HighTierTokenMaxAntagAlive,
                TokenCvars.HighTierTokenChance,
                TokenCvars.HighTierTokenMinSecAlive,
                BlockingRules:
                [
                    "Revolutionary", "Heretic", "CosmicCult", "Zombie", "LoneOpsSpawn", "NinjaSpawn", "Honkops",
                    "NukeopsRule", "PiratesRule", "LoneAbductorSpawn", "DuoAbductorSpawn", "BlobRule",
                ]
            ));
    }

    private void OnRoundRestart(RoundRestartCleanupEvent _) => _lenaApi.ClearAllLockouts();

    private void OpenAntagSelection(ICommonSession session, string itemId)
    {
        if (_lenaApi.IsTokenLockedOut(session.UserId, itemId))
        {
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), session, PopupType.Medium);
            return;
        }

        _euiManager.OpenEui(new AntagSelectionEui(itemId), session);
    }

    private void OpenCosmeticSelection(ICommonSession session, string itemId)
    {
        if (_lenaApi.IsTokenLockedOut(session.UserId, itemId))
        {
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), session, PopupType.Medium);
            return;
        }

        var ent = session.AttachedEntity;
        if (ent == null || _entMan.HasComponent<GhostComponent>(ent.Value) || !_mobState.IsAlive(ent.Value))
        {
            _popup.PopupCursor(Loc.GetString("reserve-token-alive-only"), session, PopupType.Medium);
            return;
        }

        _euiManager.OpenEui(new CosmeticSelectionEui(itemId), session);
    }
}
