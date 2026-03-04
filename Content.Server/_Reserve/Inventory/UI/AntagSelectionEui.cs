// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Content.Server.Antag;
using Content.Server.Antag.Components;
using Content.Server.Chat.Managers;
using Content.Server.EUI;
using Content.Server.GameTicking;
using Content.Server.Ghost.Roles;
using Content.Server._Reserve.LenaApi;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Popups;
using Content.Server.StationEvents.Components;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;
using Content.Shared.GameTicking.Components;
using Content.Shared.Ghost;
using Content.Shared.Ghost.Roles.Components;
using Content.Shared.Mind.Components;
using Content.Shared.Mindshield.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._Reserve.Inventory.UI;

public sealed class AntagSelectionEui : BaseEui
{
    private readonly LenaApiManager _lenaApi;
    private readonly AntagSelectionSystem _antagSelection;
    private readonly GameTicker _gameTicker;
    private readonly GhostRoleSystem _ghostRoles;
    private readonly IEntityManager _entMan;
    private readonly MobStateSystem _mobState;
    private readonly SharedRoleSystem _roles;
    private readonly PopupSystem _popup;
    private readonly IConfigurationManager _cfg;
    private readonly IRobustRandom _random;
    private readonly IChatManager _chat;
    private readonly string _itemId;
    private readonly ISawmill _sawmill;

    public AntagSelectionEui(string itemId)
    {
        _itemId = itemId;
        _lenaApi = IoCManager.Resolve<LenaApiManager>();
        _cfg = IoCManager.Resolve<IConfigurationManager>();
        _random = IoCManager.Resolve<IRobustRandom>();
        _chat = IoCManager.Resolve<IChatManager>();
        _entMan = IoCManager.Resolve<IEntityManager>();
        var sysMan = IoCManager.Resolve<IEntitySystemManager>();
        _antagSelection = sysMan.GetEntitySystem<AntagSelectionSystem>();
        _gameTicker = sysMan.GetEntitySystem<GameTicker>();
        _mobState = sysMan.GetEntitySystem<MobStateSystem>();
        _roles = sysMan.GetEntitySystem<SharedRoleSystem>();
        _popup = sysMan.GetEntitySystem<PopupSystem>();
        _ghostRoles = sysMan.GetEntitySystem<GhostRoleSystem>();
        _sawmill = Logger.GetSawmill("lena-api");
    }

    private bool IsPlayerAlive()
    {
        var entity = Player.AttachedEntity;
        if (entity == null)
            return false;
        if (_entMan.HasComponent<GhostComponent>(entity.Value))
            return false;
        if (!_entMan.TryGetComponent<MobStateComponent>(entity.Value, out var mobState))
            return false;
        return _mobState.IsAlive(entity.Value, mobState);
    }

    public override void Opened()
    {
        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        var isAlive = IsPlayerAlive();
        var allRules = _lenaApi.GetAntagRules(_itemId);
        var entries = new List<AntagRuleEntry>();
        foreach (var (ruleId, config) in allRules)
        {
            if (config.ForAlive == isAlive)
                entries.Add(new AntagRuleEntry(ruleId, config.DisplayName, config.ForAlive));
        }

        return new AntagSelectionEuiState { Rules = entries };
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        switch (msg)
        {
            case AntagSelectionEuiMsg.Close:
                Close();
                break;
            case AntagSelectionEuiMsg.SelectRule selectRule:
                TrySelectRule(selectRule.RuleId);
                break;
        }
    }

    private async void TrySelectRule(string ruleId)
    {
        if (!_lenaApi.TryBeginTokenUse(Player.UserId, _itemId))
            return;

        try
        {
            await TrySelectRuleInternal(ruleId);
        }
        finally
        {
            _lenaApi.EndTokenUse(Player.UserId, _itemId);
        }
    }

    private async Task TrySelectRuleInternal(string ruleId)
    {
        if (_lenaApi.IsTokenLockedOut(Player.UserId, _itemId))
        {
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), Player, PopupType.Medium);
            Close();
            return;
        }

        var user = _lenaApi.GetUser(Player.UserId);
        if (user == null)
        {
            Close();
            return;
        }

        var inventoryResult = await _lenaApi.GetInventoryFromApi(user.Id);
        if (!inventoryResult.IsSuccess || inventoryResult.Value == null)
        {
            Close();
            return;
        }

        user.UsableItems = inventoryResult.Value.Items
            .ConvertAll(e => e.Item)
            .FindAll(i => i.CanBeUsedIngame);

        if (user.UsableItems.All(i => i.ItemId != _itemId))
        {
            _lenaApi.NotifyItemRemoved(Player.UserId, _itemId);
            Close();
            return;
        }

        var conditions = _lenaApi.GetTokenConditions(_itemId);
        if (conditions == null)
        {
            Close();
            return;
        }

        var aliveCount = 0;
        var aliveAntagCount = 0;
        var mindShieldCount = 0;
        var query = _entMan.EntityQueryEnumerator<ActorComponent, MobStateComponent>();
        while (query.MoveNext(out var uid, out _, out var mobState))
        {
            if (!_mobState.IsAlive(uid, mobState))
                continue;
            aliveCount++;
            if (_entMan.HasComponent<MindShieldComponent>(uid))
                mindShieldCount++;
            if (_entMan.TryGetComponent<MindContainerComponent>(uid, out var mindContainer)
                && _roles.MindIsAntagonist(mindContainer.Mind))
                aliveAntagCount++;
        }

        var entity = Player.AttachedEntity;
        if (entity != null && _entMan.HasComponent<MindShieldComponent>(entity.Value))
        {
            _lenaApi.LockOutToken(Player.UserId, _itemId);
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), Player, PopupType.Medium);
            Close();
            return;
        }

        var failed = false;
        if (aliveCount < _cfg.GetCVar(conditions.MinAlive))
            failed = true;
        else if (aliveAntagCount > _cfg.GetCVar(conditions.MaxAntags))
            failed = true;
        else if (conditions.MinSec != null && mindShieldCount < _cfg.GetCVar(conditions.MinSec))
            failed = true;
        else if (conditions.BlockingRules?.Count > 0)
        {
            var ruleQuery = _entMan.EntityQueryEnumerator<ActiveGameRuleComponent, MetaDataComponent>();
            while (ruleQuery.MoveNext(out _, out _, out var meta))
            {
                if (meta.EntityPrototype?.ID is { } pid && conditions.BlockingRules.Contains(pid))
                {
                    failed = true;
                    break;
                }
            }
        }

        if (!failed && !_random.Prob(_cfg.GetCVar(conditions.Chance)))
            failed = true;

        if (failed)
        {
            _lenaApi.LockOutToken(Player.UserId, _itemId);
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), Player, PopupType.Medium);
            Close();
            return;
        }

        var usedItem = user.UsableItems.First(i => i.ItemId == _itemId);

        var rules = _lenaApi.GetAntagRules(_itemId);
        if (IsPlayerAlive()
            && rules.TryGetValue(ruleId, out var ruleConfig)
            && ruleConfig.ForAliveAction != null)
        {
            ruleConfig.ForAliveAction(Player);
        }
        else
        {
            MakeAntagForGhost(ruleId);
        }

        var displayName = rules.TryGetValue(ruleId, out var selectedRule) ? selectedRule.DisplayName : ruleId;
        _sawmill.Info(
            $"[Token] {Player.Name} ({Player.UserId}) использовал токен '{_itemId}', выбрав правило '{ruleId}'.");
        _chat.SendAdminAnnouncement(Loc.GetString("reserve-token-used",
            ("playerName", Player.Name),
            ("tokenType", _itemId),
            ("chosenAntag", displayName)));

        await _lenaApi.TakeItemFromApi(user.Id, usedItem.Id);
        user.UsableItems.RemoveAll(i => i.ItemId == _itemId);
        _lenaApi.LockOutPlayerGlobally(Player.UserId);
        _lenaApi.NotifyItemRemoved(Player.UserId, _itemId);

        Close();
    }

    private void MakeAntagForGhost(string ruleId)
    {
        var ruleEnt = _gameTicker.AddGameRule(ruleId);
        if (_entMan.HasComponent<LoadMapRuleComponent>(ruleEnt))
            _entMan.RemoveComponent<LoadMapRuleComponent>(ruleEnt);

        if (_entMan.TryGetComponent<AntagSelectionComponent>(ruleEnt, out var antagComp))
        {
            antagComp.AssignmentComplete = true;
            _gameTicker.StartGameRule(ruleEnt);

            if (antagComp.Definitions.Count > 0)
            {
                var def = antagComp.Definitions[0];
                if (def.PickPlayer)
                {
                    _antagSelection.MakeAntag((ruleEnt, antagComp), Player, def);
                }
                else
                {
                    var existingRoles = new HashSet<EntityUid>();
                    foreach (var r in _ghostRoles.GhostRoles)
                    {
                        existingRoles.Add(r.Owner);
                    }

                    _antagSelection.MakeAntag((ruleEnt, antagComp), null, def);

                    foreach (var role in _ghostRoles.GhostRoles)
                    {
                        if (existingRoles.Contains(role.Owner))
                            continue;
                        _ghostRoles.Takeover(Player, role.Comp.Identifier);
                        return;
                    }
                }
            }
        }
        else if (_entMan.TryGetComponent<RandomEntityStorageSpawnRuleComponent>(ruleEnt, out var spawnComp))
        {
            _gameTicker.StartGameRule(ruleEnt);

            var query = _entMan.EntityQueryEnumerator<GhostRoleComponent, MetaDataComponent>();
            while (query.MoveNext(out var uid, out var ghostRole, out var meta))
            {
                if (meta.EntityPrototype?.ID != spawnComp.Prototype.Id)
                    continue;
                if (_entMan.TryGetComponent<MindContainerComponent>(uid, out var mindCont) && mindCont.HasMind)
                    continue;

                _ghostRoles.GhostRoleInternalCreateMindAndTransfer(Player, uid, uid, ghostRole);
                break;
            }
        }
        else
        {
            _gameTicker.StartGameRule(ruleEnt);
        }
    }
}
