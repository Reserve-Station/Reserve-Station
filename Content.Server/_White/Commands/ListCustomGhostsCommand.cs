// SPDX-FileCopyrightText: 2025 Kirill <kirill@example.com>
// SPDX-FileCopyrightText: 2025 RedFoxIV <38788538+RedFoxIV@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Database;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Content.Shared._White.CustomGhostSystem;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Robust.Shared.Console;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._White.Commands;



[AnyCommand]
public sealed class ListCustomGhostsCommand : IConsoleCommand
{
    // [Dependency] private readonly IServerDbManager _db = default!; // Reserve edit: Fix warnings
    [Dependency] private readonly IPrototypeManager _proto = default!;
    // [Dependency] private readonly IServerPreferencesManager _prefMan = default!; // Reserve edit: Fix warnings
    // [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!; // Reserve edit: Fix warnings

    public string Command => "listcustomghosts";
    public string Description => Loc.GetString("listcustomghosts-command-description");
    public string Help => Loc.GetString("listcustomghosts-command-help-text");

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var protos = _proto.EnumeratePrototypes<CustomGhostPrototype>();
        var sb = new StringBuilder();
        if (shell.Player is not ICommonSession player)
        {
            foreach (var proto in protos)
                sb.AppendLine(proto.ID);

            shell.WriteLine(sb.ToString());
            return;
        }

        if (args.Length > 2 || args.Length == 1 && args[0] != "all")
        {
            shell.WriteLine(Help);
            return;
        }

        bool all = args.Length == 1;

        sb.AppendLine(Loc.GetString($"listcustomghosts-{(all ? "all" : "available")}-ghosts"));



        foreach(var proto in protos)
        {
            bool available = true;
            if(proto.Restrictions is not null)
                foreach(var restriction in proto.Restrictions)
                {
                    if (restriction.CanUse(player, out _))
                        continue;
                    if (restriction.HideOnFail)
                        goto skipPrototype; // wojaks_pointing.png
                    available = false;
                    break;
                }

            if (available)
                sb.AppendLine($"- {proto.ID}");
            else if (all)
                sb.AppendLine($"- {proto.ID} {Loc.GetString("listcustomghosts-locked")}");
            skipPrototype:;
        }

        shell.WriteLine(sb.ToString());
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args) =>
        args.Length switch
        {
            1 => CompletionResult.FromHint("all"),
            _ => CompletionResult.Empty
        };
}
