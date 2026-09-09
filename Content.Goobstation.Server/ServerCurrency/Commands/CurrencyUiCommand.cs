// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.ServerCurrency.UI;
using Content.Server.EUI;
using Content.Shared._Reserve.LenaApi; // Reserve-LenaApi
using Robust.Shared.Configuration; // Reserve-LenaApi
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Goobstation.Server.ServerCurrency.Commands
{
    [AnyCommand]
    public sealed class CurrencyUiCommand : IConsoleCommand
    {
        [Dependency] private readonly IConfigurationManager _configuration = default!; // Reserve-LenaApi

        public string Command => "balanceui";
        public string Description => "Open the currency UI";
        public string Help => $"{Command}";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            // Reserve-LenaApi-start
            var apiIntegrationEnabled = _configuration.GetCVar(LenaApiCVars.ApiIntegration);
            if (apiIntegrationEnabled)
            {
                shell.WriteLine(Loc.GetString("reserve-command-disabled-due-integration-enabled"));
                return;
            }
            // Reserve-LenaApi-end

            var player = shell.Player;
            if (player == null)
            {
                shell.WriteLine("This does not work from the server console.");
                return;
            }

            var eui = IoCManager.Resolve<EuiManager>();
            var ui = new CurrencyEui();
            eui.OpenEui(ui, player);
        }
    }
}
