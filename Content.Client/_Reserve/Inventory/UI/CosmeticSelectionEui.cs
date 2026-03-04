// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Eui;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;

namespace Content.Client._Reserve.Inventory.UI;

public sealed class CosmeticSelectionEui : BaseEui
{
    private CosmeticSelectionWindow? _window;

    public override void Opened()
    {
        _window = new CosmeticSelectionWindow();
        _window.OnSelectItem += protoId => SendMessage(new CosmeticSelectionEuiMsg.SelectItem { ProtoId = protoId });
        _window.OnClose += () => SendMessage(new CosmeticSelectionEuiMsg.Close());
        _window.OpenCentered();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is CosmeticSelectionEuiState cosmeticState)
            _window?.Populate(cosmeticState);
    }

    public override void Closed()
    {
        _window?.Dispose();
        _window = null;
    }
}
