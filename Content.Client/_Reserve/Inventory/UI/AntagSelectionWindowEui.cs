// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Eui;
using Content.Shared._Reserve.Inventory.UI;
using Content.Shared.Eui;

namespace Content.Client._Reserve.Inventory.UI;

public sealed class AntagSelectionEui : BaseEui
{
    private readonly AntagSelectionWindow _window;

    public AntagSelectionEui()
    {
        _window = new AntagSelectionWindow();
        _window.OnClose += () => SendMessage(new AntagSelectionEuiMsg.Close());
        _window.OnSelectRule += ruleId => SendMessage(new AntagSelectionEuiMsg.SelectRule { RuleId = ruleId });
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
        if (state is AntagSelectionEuiState s)
            _window.Populate(s);
    }
}
