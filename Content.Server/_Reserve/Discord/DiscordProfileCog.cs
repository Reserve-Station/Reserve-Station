// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Administration.Notes;
using Content.Server.Database;
using Content.Goobstation.Common.ServerCurrency;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.CCVar;
using Content.Shared.Humanoid.Prototypes;
using Content.Goobstation.Shared.ServerCurrency;
using NetCord;
using NetCord.Rest;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Server._Reserve.Discord;

public sealed class DiscordProfileCog
{
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IServerDbManager _database = default!;
    [Dependency] private readonly ICommonCurrencyManager _currency = default!;
    [Dependency] private readonly IAdminNotesManager _notes = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;

    // Patron/booster Discord roles shown next to a player's CKey, ordered highest-prestige first.
    private static readonly CVarDef<string>[] BadgeRoleCVars =
    [
        CCVars.DiscordPatronRoleHonorary,
        CCVars.DiscordPatronRoleTierX,
        CCVars.DiscordPatronRoleTier5,
        CCVars.DiscordPatronRoleTier4,
        CCVars.DiscordPatronRoleTier3,
        CCVars.DiscordPatronRoleTier2,
        CCVars.DiscordPatronRoleTier1,
        CCVars.DiscordBoosterRole,
    ];

    private static readonly Dictionary<Gender, string> GenderPronouns = new()
    {
        { Gender.Male, Loc.GetString("humanoid-profile-editor-pronouns-male-text") },
        { Gender.Female, Loc.GetString("humanoid-profile-editor-pronouns-female-text") },
        { Gender.Epicene, Loc.GetString("humanoid-profile-editor-pronouns-epicene-text") },
        { Gender.Neuter, Loc.GetString("humanoid-profile-editor-pronouns-neuter-text") },
    };

    public DiscordProfileCog()
    {
        IoCManager.InjectDependencies(this);
    }

    public Task<InteractionCallback> HandleAsync(string command, string? player, ulong discordUserId, bool ephemeral, RestClient client, ulong guildId)
    {
        var part = command switch
        {
            "profile" => ProfilePart.All,
            "balance" => ProfilePart.Balance,
            "characters" => ProfilePart.Characters,
            "inventory" => ProfilePart.Inventory,
            _ => throw new InvalidOperationException($"Unknown profile command: {command}"),
        };

        return BuildResponse(player, part, discordUserId, ephemeral, client, guildId);
    }

    private async Task<InteractionCallback> BuildResponse(string? player, ProfilePart part, ulong discordUserId, bool ephemeral, RestClient client, ulong guildId)
    {
        NetUserId userId;
        string ckey;
        ulong? discordId;

        // Security: only connected players can check other players' profiles, to prevent abuse of the bot for scraping data.
        // If the player argument is omitted, we assume the user wants their own profile.
        var linkedId = await _database.GetLinkedPlayerId(discordUserId, default);
        if (linkedId == null)
            return Respond(Loc.GetString("discord-profile-player-required"), ephemeral);

        // Security: if the player argument is included, always go ephemeral to prevent abuse of the bot for scraping data.
        if (!string.IsNullOrWhiteSpace(player))
            ephemeral = true;

        if (string.IsNullOrWhiteSpace(player))
        {
            // var linkedId = await _database.GetLinkedPlayerId(discordUserId, default);
            // if (linkedId == null)
            //     return Respond(Loc.GetString("discord-profile-player-required"), ephemeral);

            userId = linkedId.Value;
            ckey = (await _database.GetPlayerRecordByUserId(userId))?.LastSeenUserName ?? userId.ToString();
            discordId = discordUserId;
        }
        else if (TryParseUserId(player, out userId))
        {
            ckey = (await _database.GetPlayerRecordByUserId(userId))?.LastSeenUserName ?? player;
            discordId = await _database.GetLinkedDiscordId(userId.UserId, default);
        }
        else
        {
            var located = await _playerLocator.LookupIdByNameOrIdAsync(player);
            if (located == null)
                return Respond(Loc.GetString("discord-profile-player-not-found", ("player", player)), ephemeral);

            userId = located.UserId;
            ckey = located.Username;
            discordId = await _database.GetLinkedDiscordId(userId.UserId, default);
        }

        var title = await BuildTitle(ckey, discordId, client, guildId);
        var preferences = await _database.GetPlayerPreferencesAsync(userId, default);
        var description = new StringBuilder();

        if (part is ProfilePart.All or ProfilePart.Characters)
            AppendCharacters(description, preferences);
        if (part is ProfilePart.All or ProfilePart.Balance)
        {
            description.AppendLine($"### {Loc.GetString("discord-profile-balance")}");
            description.AppendLine(_currency.Stringify(_currency.GetBalance(userId)));
        }
        if (part is ProfilePart.All or ProfilePart.Inventory)
            await AppendInventory(description, userId);

        return Respond(new EmbedProperties
        {
            Title = title,
            Description = description.Length == 0 ? Loc.GetString("discord-profile-no-data") : description.ToString(),
            Color = new NetCord.Color(_configuration.GetCVar(CCVars.DiscordEmbedColor)),
        }, ephemeral);
    }

    private static InteractionCallback Respond(string content, bool ephemeral)
    {
        return InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = content,
            Flags = ephemeral ? MessageFlags.Ephemeral : null,
        });
    }

    private static InteractionCallback Respond(EmbedProperties embed, bool ephemeral)
    {
        return InteractionCallback.Message(new InteractionMessageProperties
        {
            Embeds = [embed],
            Flags = ephemeral ? MessageFlags.Ephemeral : null,
        });
    }

    // Builds the "Profile: <discord server name> (<ckey>), <badges>" title, or a "(not connected)" fallback.
    private async Task<string> BuildTitle(string ckey, ulong? discordId, RestClient client, ulong guildId)
    {
        if (discordId == null)
            return Loc.GetString("discord-profile-title-not-connected", ("player", ckey));

        GuildUser member;
        try
        {
            member = await client.GetGuildUserAsync(guildId, discordId.Value);
        }
        catch (RestException)
        {
            // Linked, but not a member of the guild (left, banned, etc.) or lookup otherwise failed.
            return Loc.GetString("discord-profile-title-not-connected", ("player", ckey));
        }

        var serverName = member.Nickname ?? member.GlobalName ?? member.Username;

        var ownedBadgeIds = BadgeRoleCVars
            .Select(cvar => ulong.TryParse(_configuration.GetCVar(cvar), out var roleId) ? roleId : (ulong?) null)
            .Where(roleId => roleId != null && member.RoleIds.Contains(roleId.Value))
            .Select(roleId => roleId!.Value)
            .ToArray();

        var badges = string.Empty;
        if (ownedBadgeIds.Length > 0)
        {
            var roles = await client.GetGuildRolesAsync(guildId);
            var roleNames = roles.ToDictionary(role => role.Id, role => role.Name);

            var sb = new StringBuilder();
            foreach (var roleId in ownedBadgeIds)
            {
                if (roleNames.TryGetValue(roleId, out var roleName))
                    sb.Append($", **{roleName}**");
            }

            badges = sb.ToString();
        }

        return Loc.GetString("discord-profile-title", ("player", serverName), ("ckey", ckey), ("badges", badges));
    }

    private void AppendCharacters(StringBuilder output, PlayerPreferences? preferences)
    {
        output.AppendLine($"### {Loc.GetString("discord-profile-characters")}");
        if (preferences == null || preferences.Characters.Count == 0)
        {
            output.AppendLine(Loc.GetString("discord-profile-no-characters"));
            return;
        }

        foreach (var (_, profile) in preferences.Characters.OrderBy(pair => pair.Key))
        {
            if (profile is not HumanoidCharacterProfile humanoid)
                continue;

            var highJob = humanoid.JobPriorities.FirstOrDefault(pair => pair.Value == JobPriority.High).Key;
            var jobName = Loc.GetString("discord-profile-no-job");
            if (!string.IsNullOrEmpty(highJob.Id) && _prototypes.TryIndex(highJob, out JobPrototype? job))
            {
                jobName = humanoid.JobAlternateTitles.TryGetValue(highJob, out var altTitleId) &&
                          _prototypes.TryIndex(altTitleId, out JobAlternateTitlePrototype? altTitle)
                    ? altTitle.LocalizedName(humanoid.Gender)
                    : Loc.GetString(job.Name);
            }

            var speciesName = humanoid.Species;
            if (_prototypes.TryIndex<SpeciesPrototype>(humanoid.Species, out var species))
                speciesName = Loc.GetString(species.Name);

            if (!GenderPronouns.TryGetValue(humanoid.Gender, out var pronouns))
                pronouns = Loc.GetString("humanoid-profile-editor-pronouns-epicene-text");
            output.AppendLine(Loc.GetString("discord-profile-character",
                ("name", humanoid.Name),
                ("gender", pronouns),
                ("age", humanoid.Age),
                ("species", speciesName),
                ("job", jobName)));
        }
    }

    private async Task AppendInventory(StringBuilder output, NetUserId userId)
    {
        output.AppendLine($"### {Loc.GetString("discord-profile-inventory")}");
        var tokenListings = _prototypes.EnumeratePrototypes<TokenListingPrototype>().ToArray();
        var tokenMessages = tokenListings.ToDictionary(listing => Loc.GetString(listing.AdminNote));
        var remarks = await _notes.GetAllAdminRemarks(userId.UserId);
        var boughtTokens = remarks
            .Where(remark => tokenMessages.ContainsKey(remark.Message))
            .GroupBy(remark => tokenMessages[remark.Message])
            .OrderBy(group => group.Key.ID);

        if (!boughtTokens.Any())
        {
            output.AppendLine(Loc.GetString("discord-profile-no-tokens"));
            return;
        }

        foreach (var token in boughtTokens)
            output.AppendLine(Loc.GetString("discord-profile-token",
                ("token", Loc.GetString(token.Key.Label)),
                ("count", token.Count())));
    }

    private static bool TryParseUserId(string value, out NetUserId userId)
    {
        if (Guid.TryParse(value, out var guid))
        {
            userId = new NetUserId(guid);
            return true;
        }

        userId = default;
        return false;
    }

    private enum ProfilePart
    {
        All,
        Balance,
        Characters,
        Inventory,
    }
}
