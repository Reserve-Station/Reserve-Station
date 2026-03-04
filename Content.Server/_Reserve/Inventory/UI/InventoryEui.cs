// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server._Reserve.LenaApi;
using Content.Server.EUI;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;

namespace Content.Server._Reserve.Inventory.UI;

public sealed class InventoryEui : BaseEui
{
    [Dependency] private readonly LenaApiManager _lenaApi = default!;

    private List<InventoryItemData>? _items;
    private bool _isLoading = true;
    private string? _errorMessage;

    public InventoryEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override void Opened()
    {
        _lenaApi.RegisterInventoryRemoveCallback(Player.UserId, RemoveItem);
        StateDirty();
        FetchInventory();
    }

    public override void Closed()
    {
        _lenaApi.UnregisterInventoryRemoveCallback(Player.UserId);
    }

    public void RemoveItem(string itemId)
    {
        _items?.RemoveAll(i => i.ItemId == itemId);
        StateDirty();
        Close();
    }

    private async void FetchInventory()
    {
        var user = _lenaApi.GetUser(Player.UserId);
        if (user == null)
        {
            _errorMessage = Loc.GetString("reserve-inventory-error-not-found");
            _isLoading = false;
            StateDirty();
            return;
        }

        var result = await _lenaApi.GetInventoryFromApi(user.Id);
        if (!result.IsSuccess || result.Value == null)
        {
            _errorMessage = Loc.GetString("reserve-inventory-error-fetch");
            _isLoading = false;
            StateDirty();
            return;
        }

        user.UsableItems = result.Value.Items
            .ConvertAll(e => e.Item)
            .FindAll(i => i.CanBeUsedIngame);

        _items = result.Value.Items
            .Where(e => e.Item.CanBeUsedIngame)
            .Select(e => new InventoryItemData(e.Item.ItemId, e.Item.ItemName, e.Item.Description, e.Item.Rarity, e.Item.CanBeUsedIngame, _lenaApi.GetItemIcon(e.Item.ItemId)))
            .ToList();
        _isLoading = false;
        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        if (_isLoading)
            return new InventoryEuiState { IsLoading = true };

        if (_errorMessage != null)
            return new InventoryEuiState { IsLoading = false, ErrorMessage = _errorMessage };

        return new InventoryEuiState
        {
            IsLoading = false,
            Items = _items,
        };
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        switch (msg)
        {
            case InventoryEuiMsg.Close:
                Close();
                break;
            case InventoryEuiMsg.UseItem useItem:
                _lenaApi.TryUseItem(Player, useItem.ItemId);
                break;
        }
    }
}
