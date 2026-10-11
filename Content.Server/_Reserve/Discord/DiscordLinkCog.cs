// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Content.Server._Reserve.Discord.AdminGuard;
using Content.Server.Database;
using NetCord;
using NetCord.Rest;
using Robust.Shared.Network;
using System.Linq;

namespace Content.Server._Reserve.Discord;

public sealed partial class DiscordLinkCog
{
    [Dependency] private IServerDbManager _database = default!;
    [Dependency] private IAdminGuardManager _adminGuard = default!;

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

        if (playerId is { } linkedId)
            _adminGuard.DiscordAccountLinked(linkedId, discordUserId);

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
        var playerId = await _database.GetLinkedPlayerId(discordUserId, default);
        var unlinked = await _database.UnlinkDiscordAccount(discordUserId, default);
        if (unlinked)
        {
            _adminGuard.DiscordAccountUnlinked(playerId, discordUserId);
            return Ephemeral(Loc.GetString("discord-unlink-success"));
        }

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

    /// <summary>
    ///     Dynamically assigns a Patron Tier to a player based on their Discord account.
    ///     First, checks if the player has a linked Discord account and then assigns the appropriate Patron Tier based on their Discord roles.
    /// </summary>
    public async Task AssignPatronTierAsync(NetUserId playerId, RestClient client, ulong guildId)
    {
        var discordId = await _database.GetLinkedDiscordId(playerId, default);

        if (discordId == null)
            return;

        GuildUser member;
        try
        {
            member = await client.GetGuildUserAsync(guildId, discordId.Value);
        }
        catch (RestException)
        {
            // Linked, but not a member of the guild (left, banned, etc.) or lookup otherwise failed.
            return;
        }

        // Get all patron tiers
        var patronTiers = await _database.GetPatronTiers();

        foreach (var patronTier in patronTiers)
        {
            if (member.RoleIds.Contains(patronTier.DiscordRole))
            {
                await _database.SetPatron(playerId, patronTier.Id);
                return;
            }
        }
        // No matching patron tier found or member does not have any patron roles.
        await _database.SetPatron(playerId, null);
    }
}
