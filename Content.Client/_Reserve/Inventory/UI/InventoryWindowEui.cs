// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Eui;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;

namespace Content.Client._Reserve.Inventory.UI;

public sealed class InventoryEui : BaseEui
{
    private readonly InventoryWindow _window;

    public InventoryEui()
    {
        _window = new InventoryWindow();
        _window.OnClose += () => SendMessage(new InventoryEuiMsg.Close());
        _window.OnUseItem += itemId =>
        {
            SendMessage(new InventoryEuiMsg.UseItem { ItemId = itemId });
            _window.Close();
        };
    }

    public override void Opened()
    {
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is InventoryEuiState s)
            _window.Populate(s);
    }
}
