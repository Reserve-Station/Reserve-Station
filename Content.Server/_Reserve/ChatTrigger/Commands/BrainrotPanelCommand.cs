using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._Reserve.ChatTrigger;

[AdminCommand(AdminFlags.Admin)]
public sealed class BrainrotPanelCommand : IConsoleCommand
{
    public string Command => "brainrotpanel";
    public string Description => "Opens the brainrot panel to manage custom triggers.";
    public string Help => "brainrotpanel";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        var euiManager = IoCManager.Resolve<EuiManager>();
        euiManager.OpenEui(new BrainrotPanelEui(), player);
    }
}
