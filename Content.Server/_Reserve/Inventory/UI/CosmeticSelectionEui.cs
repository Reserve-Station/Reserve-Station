// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Content.Server.Chat.Managers;
using Content.Server.EUI;
using Content.Server._Reserve.LenaApi;
using Content.Server.Popups;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server._Reserve.Inventory.UI;

public sealed class CosmeticSelectionEui : BaseEui
{
    private readonly LenaApiManager _lenaApi;
    private readonly IEntityManager _entMan;
    private readonly IPrototypeManager _proto;
    private readonly SharedHandsSystem _hands;
    private readonly IChatManager _chat;
    private readonly PopupSystem _popup;
    private readonly ISawmill _sawmill;
    private readonly string _itemId;

    public CosmeticSelectionEui(string itemId)
    {
        _itemId = itemId;
        _lenaApi = IoCManager.Resolve<LenaApiManager>();
        _entMan = IoCManager.Resolve<IEntityManager>();
        _proto = IoCManager.Resolve<IPrototypeManager>();
        _chat = IoCManager.Resolve<IChatManager>();
        var sysMan = IoCManager.Resolve<IEntitySystemManager>();
        _hands = sysMan.GetEntitySystem<SharedHandsSystem>();
        _popup = sysMan.GetEntitySystem<PopupSystem>();
        _sawmill = Logger.GetSawmill("lena-api");
    }

    public override void Opened() => StateDirty();

    public override EuiStateBase GetNewState()
    {
        return new CosmeticSelectionEuiState
        {
            ProtoIds = _lenaApi.GetCosmeticItems(_itemId).ToList(),
        };
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        switch (msg)
        {
            case CosmeticSelectionEuiMsg.SelectItem select:
                TrySelectItem(select.ProtoId);
                break;
            case CosmeticSelectionEuiMsg.Close:
                Close();
                break;
        }
    }

    private async void TrySelectItem(string protoId)
    {
        if (!_lenaApi.TryBeginTokenUse(Player.UserId, _itemId))
            return;

        try
        {
            await TrySelectItemInternal(protoId);
        }
        finally
        {
            _lenaApi.EndTokenUse(Player.UserId, _itemId);
        }
    }

    private async Task TrySelectItemInternal(string protoId)
    {
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

        user.UsableItems = inventoryResult.Value.Items.ConvertAll(e => e.Item).FindAll(i => i.CanBeUsedIngame);

        if (user.UsableItems.All(i => i.ItemId != _itemId))
        {
            _lenaApi.NotifyItemRemoved(Player.UserId, _itemId);
            Close();
            return;
        }

        if (!_lenaApi.GetCosmeticItems(_itemId).Contains(protoId))
        {
            Close();
            return;
        }

        var playerEnt = Player.AttachedEntity;
        if (playerEnt == null)
        {
            Close();
            return;
        }

        var usedItem = user.UsableItems.First(i => i.ItemId == _itemId);
        var takeResult = await _lenaApi.TakeItemFromApi(user.Id, usedItem.Id);
        if (!takeResult.IsSuccess)
        {
            _sawmill.Error(
                $"[Token] Не удалось снять токен '{_itemId}' через API для {Player.Name} ({Player.UserId}): {takeResult.Error}");
            _popup.PopupCursor(Loc.GetString("reserve-token-use-failed"), Player, PopupType.Medium);
            Close();
            return;
        }

        var displayName = _proto.TryIndex<EntityPrototype>(protoId, out var prototype)
            ? prototype.Name
            : protoId;

        _sawmill.Info(
            $"[Token] {Player.Name} ({Player.UserId}) использовал токен '{_itemId}', выбрав предмет '{protoId}'.");
        _chat.SendAdminAnnouncement(Loc.GetString("reserve-cosmetic-token-used",
            ("playerName", Player.Name),
            ("tokenType", _itemId),
            ("chosenItem", displayName)));

        var coords = _entMan.GetComponent<TransformComponent>(playerEnt.Value).Coordinates;
        var spawnedEnt = _entMan.SpawnEntity(protoId, coords);
        _hands.TryPickupAnyHand(playerEnt.Value, spawnedEnt);

        user.UsableItems.RemoveAll(i => i.ItemId == _itemId);
        _lenaApi.LockOutPlayerGlobally(Player.UserId);
        _lenaApi.NotifyItemRemoved(Player.UserId, _itemId);

        Close();
    }
}
