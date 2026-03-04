// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Reserve.Inventory.UI;
using Content.Server.EUI;
using Content.Shared._Reserve.LenaApi;
using Content.Shared.Administration;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server._Reserve.Inventory.Commands;

[AnyCommand]
public sealed class InventoryUiCommand : IConsoleCommand
{
    [Dependency] private readonly IConfigurationManager _configuration = default!;

    public string Command => "inventory";
    public string Description => "Open the inventory UI";
    public string Help => $"{Command}";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var apiIntegrationEnabled = _configuration.GetCVar(LenaApiCVars.ApiIntegration);
        if (!apiIntegrationEnabled)
        {
            shell.WriteLine(Loc.GetString("reserve-command-requires-integration-enabled"));
            return;
        }

        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("This does not work from the server console.");
            return;
        }

        var eui = IoCManager.Resolve<EuiManager>();
        var ui = new InventoryEui();
        eui.OpenEui(ui, player);
    }
}
