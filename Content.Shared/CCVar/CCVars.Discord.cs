// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     The role that will get mentioned if a new SOS ahelp comes in.
    /// </summary>
    public static readonly CVarDef<string> DiscordAhelpMention =
        CVarDef.Create("discord.on_call_ping", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     URL of the discord webhook to relay unanswered ahelp messages.
    /// </summary>
    public static readonly CVarDef<string> DiscordOnCallWebhook =
        CVarDef.Create("discord.on_call_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     URL of the Discord webhook which will relay all ahelp messages.
    /// </summary>
    public static readonly CVarDef<string> DiscordAHelpWebhook =
        CVarDef.Create("discord.ahelp_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     The server icon to use in the Discord ahelp embed footer.
    ///     Valid values are specified at https://discord.com/developers/docs/resources/channel#embed-object-embed-footer-structure.
    /// </summary>
    public static readonly CVarDef<string> DiscordAHelpFooterIcon =
        CVarDef.Create("discord.ahelp_footer_icon", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     The avatar to use for the webhook. Should be an URL.
    /// </summary>
    public static readonly CVarDef<string> DiscordAHelpAvatar =
        CVarDef.Create("discord.ahelp_avatar", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord webhook which will relay all custom votes. If left empty, disables the webhook.
    /// </summary>
    public static readonly CVarDef<string> DiscordVoteWebhook =
        CVarDef.Create("discord.vote_webhook", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord webhook which will relay all votekick votes. If left empty, disables the webhook.
    /// </summary>
    public static readonly CVarDef<string> DiscordVotekickWebhook =
        CVarDef.Create("discord.votekick_webhook", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord webhook which will relay round restart messages.
    /// </summary>
    public static readonly CVarDef<string> DiscordRoundUpdateWebhook =
        CVarDef.Create("discord.round_update_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Role id for the Discord webhook to ping when the round ends.
    /// </summary>
    public static readonly CVarDef<string> DiscordRoundEndRoleWebhook =
        CVarDef.Create("discord.round_end_role", string.Empty, CVar.SERVERONLY);


    /// <summary>
    ///     The token used to authenticate with Discord. For the Bot to function set: discord.token, discord.guild_id, and discord.prefix.
    ///     If this is empty, the bot will not connect.
    /// </summary>
    public static readonly CVarDef<string> DiscordToken =
        CVarDef.Create("discord.token", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     The Discord guild ID to use for commands as well as for several other features.
    ///     If this is empty, the bot will not connect.
    /// </summary>
    public static readonly CVarDef<string> DiscordGuildId =
        CVarDef.Create("discord.guild_id", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Prefix used for commands for the Discord bot.
    ///     If this is empty, the bot will not connect.
    /// </summary>
    public static readonly CVarDef<string> DiscordPrefix =
        CVarDef.Create("discord.prefix", "!", CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord webhook which will relay watchlist connection notifications. If left empty, disables the webhook.
    /// </summary>
    public static readonly CVarDef<string> DiscordWatchlistConnectionWebhook =
        CVarDef.Create("discord.watchlist_connection_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     How long to buffer watchlist connections for, in seconds.
    ///     All connections within this amount of time from the first one will be batched and sent as a single
    ///     Discord notification. If zero, always sends a separate notification for each connection (not recommended).
    /// </summary>
    public static readonly CVarDef<float> DiscordWatchlistConnectionBufferTime =
        CVarDef.Create("discord.watchlist_connection_buffer_time", 5f, CVar.SERVERONLY);

    /// <summary>
    /// URL of the Discord webhook which will relay bans info to the channel.
    /// </summary>
    public static readonly CVarDef<string> DiscordBansWebhook =
        CVarDef.Create("discord.bans_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Reserve - ADT port
    ///     URL of the Discord adminchat info to the channel.
    /// </summary>
    public static readonly CVarDef<string> DiscordAdminchatWebhook =
        CVarDef.Create("discord.adminchat_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL | CVar.ARCHIVE);

    /// <summary>
    ///     URL of the Discord webhook which will receive station news acticles at the round end.
    ///     If left empty, disables the webhook.
    /// </summary>
    public static readonly CVarDef<string> DiscordNewsWebhook =
        CVarDef.Create("discord.news_webhook", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     HEX color of station news discord webhook's embed.
    /// </summary>
    public static readonly CVarDef<string> DiscordNewsWebhookEmbedColor =
        CVarDef.Create("discord.news_webhook_embed_color", Color.LawnGreen.ToHex(), CVar.SERVERONLY);

    /// <summary>
    ///     Whether or not articles should be sent mid-round instead of all at once at the round's end
    /// </summary>
    public static readonly CVarDef<bool> DiscordNewsWebhookSendDuringRound =
        CVarDef.Create("discord.news_webhook_send_during_round", false, CVar.SERVERONLY);

    /// <summary>
    ///     Reserve ooc-chat
    ///     URL of the Discord OOC webhook.
    /// </summary>
    public static readonly CVarDef<string> DiscordOOCChatWebhook =
        CVarDef.Create("discord.ooc_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL | CVar.ARCHIVE);

    // Reserve edit start: Full discord bot integration
    /// <summary>
    ///     Whether the "privately" option of the Discord profile commands defaults to true (ephemeral) when omitted.
    /// </summary>
    public static readonly CVarDef<bool> DiscordProfilePrivatelyDefault =
        CVarDef.Create("discord.profile_privately_default", true, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the honorary patron tier, shown next to a player's CKey in profile commands.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleHonorary =
        CVarDef.Create("discord.patron_role_honorary", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the highest patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTierX =
        CVarDef.Create("discord.patron_role_tier_x", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the 5th patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTier5 =
        CVarDef.Create("discord.patron_role_tier_5", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the 4th patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTier4 =
        CVarDef.Create("discord.patron_role_tier_4", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the 3rd patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTier3 =
        CVarDef.Create("discord.patron_role_tier_3", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the 2nd patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTier2 =
        CVarDef.Create("discord.patron_role_tier_2", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the 1st patron tier.
    /// </summary>
    public static readonly CVarDef<string> DiscordPatronRoleTier1 =
        CVarDef.Create("discord.patron_role_tier_1", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Discord role ID for the server booster role.
    /// </summary>
    public static readonly CVarDef<string> DiscordBoosterRole =
        CVarDef.Create("discord.booster_role", string.Empty, CVar.SERVERONLY);

    /// <summary>
    ///     Embed line color for embedded discord bot responses.
    /// </summary>
    public static readonly CVarDef<int> DiscordEmbedColor =
        CVarDef.Create("discord.embed_color", 0x992D22, CVar.SERVERONLY);
    // Reserve edit end: Full discord bot integration
}
