// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.Patrons;
using Content.Server._Reserve.Discord.AdminGuard;
using Content.Server.Database;
using NetCord;
using NetCord.Rest;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Server._Reserve.Discord;

public sealed partial class DiscordLinkCog
{
    [Dependency] private IServerDbManager _database = default!;
    [Dependency] private IAdminGuardManager _adminGuard = default!;
    [Dependency] private IPrototypeManager _prototypes = default!;

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
    ///     Assigns all patron tiers matching the linked Discord account's roles.
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
        var tierIds = _prototypes.EnumeratePrototypes<PatronTierPrototype>()
            .Where(tier => member.RoleIds.Contains(tier.DiscordRole))
            .Select(tier => tier.DatabaseId)
            .ToList();
        await _database.SetPatronTiers(playerId, tierIds);
    }
}
