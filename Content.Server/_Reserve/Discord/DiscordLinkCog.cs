// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Content.Server.Database;
using NetCord;
using NetCord.Rest;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord;

public sealed class DiscordLinkCog
{
    [Dependency] private readonly IServerDbManager _database = default!;

    public DiscordLinkCog()
    {
        IoCManager.InjectDependencies(this);
    }

    /// <summary>
    ///     Handles the /link command from Discord.
    ///     Returns an ephemeral message to the user with the result of the linking attempt.
    /// </summary>
    public async Task<InteractionCallback> HandleLinkAsync(string code, ulong discordUserId)
    {
        if (!Guid.TryParse(code, out var parsedCode))
            return Ephemeral(Loc.GetString("discord-link-invalid-code"));

        (LinkAccountCodeResult result, var playerId) = await _database.ConsumeLinkingCode(parsedCode, discordUserId, default);
        switch (result)
        {
            case LinkAccountCodeResult.CodeNotFound:
                return Ephemeral(Loc.GetString("discord-link-code-not-found"));
            case LinkAccountCodeResult.CodeExpired:
                return Ephemeral(Loc.GetString("discord-link-code-expired"));
            case LinkAccountCodeResult.DiscordAlreadyLinked:
                return Ephemeral(Loc.GetString("discord-link-discord-already-linked"));
        }

        var name = playerId is { } id
            ? (await _database.GetPlayerRecordByUserId(id))?.LastSeenUserName ?? id.ToString()
            : "?";

        return Ephemeral(Loc.GetString("discord-link-success", ("player", name)));
    }

    /// <summary>
    ///     Handles the /unlink command from Discord.
    ///     Returns an ephemeral message to the user with the result of the unlinking attempt.
    /// </summary>
    public async Task<InteractionCallback> HandleUnlinkAsync(ulong discordUserId)
    {
        var unlinked = await _database.UnlinkDiscordAccount(discordUserId, default);
        if (unlinked)
            return Ephemeral(Loc.GetString("discord-unlink-success"));
        else
            return Ephemeral(Loc.GetString("discord-unlink-not-linked"));
    }

    private static InteractionCallback Ephemeral(string message)
    {
        return InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = message,
            Flags = MessageFlags.Ephemeral,
        });
    }
}
