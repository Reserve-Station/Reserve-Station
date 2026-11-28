// SPDX-License-Identifier: MIT

using Content.Server._Reserve.Discord.AdminGuard; // Reserve edit: Reserve Red - AdminGuard
using Content.Server._Reserve.Discord.Logger; // Reserve edit: Reserve Red - AdminGuard
using Content.Server.Administration.Managers;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [UsedImplicitly]
    public sealed class PromoteHostCommand : LocalizedCommands
    {
        [Dependency] private readonly IAdminManager _adminManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IAdminGuardManager _adminGuard = default!; // Reserve edit: Reserve Red - AdminGuard

        public override string Command => "promotehost";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString($"shell-need-exactly-one-argument"));
                return;
            }

            if (!_playerManager.TryGetSessionByUsername(args[0], out var targetPlayer))
            {
                shell.WriteLine(Loc.GetString($"shell-target-player-does-not-exist"));
                return;
            }

            _adminManager.PromoteHost(targetPlayer);
            _adminGuard.HostPromoted(DiscordLogActor.From(targetPlayer), DiscordLogActor.From(shell.Player)); // Reserve edit: Reserve Red - AdminGuard
        }
    }
}