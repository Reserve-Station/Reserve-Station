// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.Discord;
using Content.Server.Discord.DiscordLink;
using Content.Server.GameTicking;
using Content.Shared.Administration.Notes;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server._Reserve.Discord.BanGuard;

public sealed partial class BanGuardManager : IBanGuardManager, IPostInjectInit
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private IEntitySystemManager _systems = default!;
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private DiscordLink _link = default!;
    [Dependency] private DiscordWebhook _webhook = default!;

    private const int ColorBan = 0xff0000;
    private const int ColorRoleBan = 0xffa500;
    private const int ColorUnban = 0x2ecc71;
    private const int ColorNote = 0x3498db;
    private const int ColorNoteAdded = 0x3498db;
    private const int ColorNoteModified = 0xf1c40f;
    private const int ColorNoteDeleted = 0x95a5a6;

    private const int EmbedLimit = 4000;

    private ISawmill _sawmill = default!;

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("discord.ban_guard");
    }

    #region Public API

    public void BanCreated(BanDef ban)
    {
        _ = Guarded(async () =>
        {
            var isRole = ban.Type == BanType.Role;
            var msg = NewMessage(
                isRole ? "ban-guard-title-role-ban" : "ban-guard-title-server-ban",
                isRole ? ColorRoleBan : ColorBan,
                ban.Id);

            var names = await UserLabels(ban.UserIds);
            msg.Add("ban-guard-target", ("target", TargetsText(names)));
            msg.Add("ban-guard-reason", ("reason", ban.Reason));
            AddDuration(msg, ban.BanTime, ban.ExpirationTime);

            if (isRole)
            {
                msg.Add("ban-guard-new-roles", ("roles", RolesText(ban.Roles ?? [])));
                for (var i = 0; i < ban.UserIds.Length; i++)
                {
                    await AddActiveRoles(msg, ban.UserIds[i], await PlayerName(ban.UserIds[i]));
                }
            }

            msg.Add("ban-guard-banned-by", ("admin", await AdminLabel(ban.BanningAdmin)));

            await Send(msg, true, true);
        });
    }

    public void BanPardoned(BanDef ban, NetUserId? admin)
    {
        _ = Guarded(async () =>
        {
            var isRole = ban.Type == BanType.Role;
            var msg = NewMessage(
                isRole ? "ban-guard-title-role-unban" : "ban-guard-title-server-unban",
                ColorUnban,
                ban.Id);

            msg.Add("ban-guard-target", ("target", TargetsText(await UserLabels(ban.UserIds))));
            msg.Add("ban-guard-reason", ("reason", ban.Reason));

            if (isRole)
                msg.Add("ban-guard-pardoned-roles", ("roles", RolesText(ban.Roles ?? [])));

            msg.Add("ban-guard-unbanned-by", ("admin", await AdminLabel(admin)));

            await Send(msg, true, true);
        });
    }

    public void NoteAdded(SharedAdminNote note, NetUserId createdBy)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-note-added", ColorNoteAdded, note.Id, ("type", NoteTypeName(note.NoteType)));
            await AddNoteHeader(msg, note);
            msg.Add("ban-guard-note-message", ("message", note.Message));
            AddNoteDetails(msg, note);
            msg.Add("ban-guard-note-expiry", ("expiry", ExpiryText(note.ExpiryTime)));
            msg.Add("ban-guard-note-by", ("admin", await AdminLabel(createdBy)));

            await Send(msg, false, true);
        });
    }

    public void NoteModified(SharedAdminNote oldNote, SharedAdminNote newNote, NetUserId editedBy)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-note-modified", ColorNoteModified, newNote.Id, ("type", NoteTypeName(newNote.NoteType)));
            await AddNoteHeader(msg, newNote);

            if (oldNote.Message != newNote.Message)
            {
                msg.Add("ban-guard-note-message-changed",
                    ("old", oldNote.Message),
                    ("new", newNote.Message));
            }

            if (oldNote.NoteSeverity != newNote.NoteSeverity)
            {
                msg.Add("ban-guard-note-severity-changed",
                    ("old", SeverityText(oldNote.NoteSeverity)),
                    ("new", SeverityText(newNote.NoteSeverity)));
            }

            if (oldNote.Secret != newNote.Secret)
                msg.Add("ban-guard-note-secret-changed", ("old", BoolText(oldNote.Secret)), ("new", BoolText(newNote.Secret)));

            if (oldNote.ExpiryTime != newNote.ExpiryTime)
            {
                msg.Add("ban-guard-note-expiry-changed",
                    ("old", ExpiryText(oldNote.ExpiryTime)),
                    ("new", ExpiryText(newNote.ExpiryTime)));
            }

            msg.Add("ban-guard-note-by", ("admin", await AdminLabel(editedBy)));

            await Send(msg, false, true);
        });
    }

    public void NoteDeleted(SharedAdminNote note, NetUserId deletedBy)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-note-deleted", ColorNoteDeleted, note.Id, ("type", NoteTypeName(note.NoteType)));
            await AddNoteHeader(msg, note);
            msg.Add("ban-guard-note-message", ("message", note.Message));
            AddNoteDetails(msg, note);
            msg.Add("ban-guard-note-by", ("admin", await AdminLabel(deletedBy)));

            await Send(msg, false, true);
        });
    }

    public void PlayerFirstJoined(string playerName, NetUserId userId, string creationDate)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-first-join", ColorNote, null);
            msg.Add("player-first-join-message-webhook", ("name", await PlayerLabel(userId, playerName)));
            msg.Add("player-first-join-account-date", ("creationDate", creationDate));
            msg.Add("ban-guard-user-id", ("userId", userId.ToString()));

            await Send(msg, false, true);
        });
    }

    public void WatchlistPlayerJoined(NetUserId player, AdminWatchlistRecord watchlist)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-watchlist", ColorNote, watchlist.Id);
            msg.Add("ban-guard-target", ("target", TargetsText(await UserLabels([player]))));
            msg.Add("ban-guard-note-message", ("message", watchlist.Message));
            msg.Add("ban-guard-note-expiry", ("expiry", watchlist.ExpirationTime is { } expiry
                ? DiscordTime(expiry.UtcDateTime)
                : Loc.GetString("ban-guard-never")));

            if (watchlist.CreatedBy is { } creator)
                msg.Add("ban-guard-note-by", ("admin", await PlayerLabel(creator.UserId, creator.LastSeenUserName)));

            await Send(msg, false, true);
        });
    }

    public void BanExemptionChanged(NetUserId player, ServerBanExemptFlags oldFlags, ServerBanExemptFlags newFlags, NetUserId? admin)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage("ban-guard-title-exemption", ColorNote, null);
            msg.Add("ban-guard-target", ("target", TargetsText(await UserLabels([player]))));
            msg.Add("ban-guard-exemption-changed", ("old", FlagsText(oldFlags)), ("new", FlagsText(newFlags)));
            msg.Add("ban-guard-note-by", ("admin", await AdminLabel(admin)));

            await Send(msg, false, true);
        });
    }

    public void JobWhitelistChanged(NetUserId player, ProtoId<JobPrototype> job, bool added, NetUserId? admin)
    {
        _ = Guarded(async () =>
        {
            var msg = NewMessage(added ? "ban-guard-title-job-whitelist-added" : "ban-guard-title-job-whitelist-removed", ColorNote, null);
            msg.Add("ban-guard-target", ("target", TargetsText(await UserLabels([player]))));
            msg.Add("ban-guard-whitelist-job", ("job", RoleName(new BanRoleDef(BanManager.DbTypeJob, job.Id))));
            msg.Add("ban-guard-note-by", ("admin", await AdminLabel(admin)));

            await Send(msg, false, true);
        });
    }

    #endregion

    #region Message building

    private BanGuardMessage NewMessage(string titleKey, int color, int? id, (string, object)? extra = null)
    {
        var args = new List<(string, object)> { ("id", id?.ToString() ?? "-") };
        if (extra is { } e)
            args.Add(e);

        return new BanGuardMessage(Loc.GetString(titleKey, args.ToArray()), color, Footer());
    }

    private string Footer()
    {
        var server = _cfg.GetCVar(CCVars.GameHostName);
        var round = string.Empty;

        if (_systems.TryGetEntitySystem<GameTicker>(out var ticker))
        {
            round = ticker.RunLevel switch
            {
                GameRunLevel.PreRoundLobby => ticker.RoundId == 0
                    ? Loc.GetString("ban-guard-round-lobby-restart")
                    : Loc.GetString("ban-guard-round-lobby", ("id", (ticker.RoundId + 1).ToString())),
                GameRunLevel.InRound => Loc.GetString("ban-guard-round-in-round", ("id", ticker.RoundId.ToString())),
                GameRunLevel.PostRound => Loc.GetString("ban-guard-round-post", ("id", ticker.RoundId.ToString())),
                _ => string.Empty,
            };
        }

        return $"{server} ({round})";
    }

    private static void AddDuration(BanGuardMessage msg, DateTimeOffset banTime, DateTimeOffset? expiration)
    {
        if (expiration is not { } expires)
        {
            msg.Add("ban-guard-permanent");
            return;
        }

        // BanTime и срок считаются в разные моменты, поэтому округляем до минуты
        var duration = TimeSpan.FromMinutes(Math.Round((expires - banTime).TotalMinutes));
        msg.Add("ban-guard-duration",
            ("days", ((int) duration.TotalDays).ToString()),
            ("hours", duration.Hours.ToString()),
            ("minutes", duration.Minutes.ToString()));
        msg.Add("ban-guard-expires", ("expires", DiscordTime(expires.UtcDateTime)));
    }

    private async Task AddActiveRoles(BanGuardMessage msg, NetUserId userId, string userName)
    {
        List<BanDef> bans;
        try
        {
            bans = await _db.GetBansAsync(null, userId, null, null, false, BanType.Role);
        }
        catch (Exception e)
        {
            // Список активных ролей - дополнительная информация, без неё сообщение всё равно отправляется
            _sawmill.Warning($"Failed to get active role bans of {userId}: {e}");
            return;
        }

        var roles = bans.SelectMany(b => b.Roles ?? []).Distinct();
        msg.Add("ban-guard-all-roles", ("target", userName), ("roles", RolesText(roles)));
    }

    private async Task AddNoteHeader(BanGuardMessage msg, SharedAdminNote note)
    {
        msg.Add("ban-guard-target", ("target", TargetsText(await UserLabels(note.Players))));
    }

    private static void AddNoteDetails(BanGuardMessage msg, SharedAdminNote note)
    {
        if (note.NoteSeverity is { } severity)
            msg.Add("ban-guard-note-severity", ("severity", SeverityText(severity)));

        // Для наблюдений и сообщений секретность задаётся типом, поэтому показываем только у заметок
        if (note.NoteType == NoteType.Note)
            msg.Add("ban-guard-note-secret", ("secret", BoolText(note.Secret)));
    }

    private string RolesText(IEnumerable<BanRoleDef> roles)
    {
        var names = roles.Select(RoleName).ToArray();
        return names.Length == 0 ? Loc.GetString("ban-guard-none") : string.Join(", ", names);
    }

    private string RoleName(BanRoleDef role)
    {
        if (role.RoleType == BanManager.DbTypeJob && _proto.TryIndex<JobPrototype>(role.RoleId, out var job))
            return job.LocalizedName;

        if (role.RoleType == BanManager.DbTypeAntag && _proto.TryIndex<AntagPrototype>(role.RoleId, out var antag))
            return Loc.GetString(antag.Name);

        return role.RoleId;
    }

    private static string SeverityText(NoteSeverity? severity)
    {
        return severity switch
        {
            NoteSeverity.None => Loc.GetString("admin-note-editor-severity-none"),
            NoteSeverity.Minor => Loc.GetString("admin-note-editor-severity-low"),
            NoteSeverity.Medium => Loc.GetString("admin-note-editor-severity-medium"),
            NoteSeverity.High => Loc.GetString("admin-note-editor-severity-high"),
            _ => "-",
        };
    }

    private static string BoolText(bool value)
    {
        return value ? ":white_check_mark:" : ":x:";
    }

    private static string NoteTypeName(NoteType type)
    {
        return Loc.GetString(type switch
        {
            NoteType.Note => "ban-guard-note-type-note",
            NoteType.Watchlist => "ban-guard-note-type-watchlist",
            NoteType.Message => "ban-guard-note-type-message",
            NoteType.ServerBan => "ban-guard-note-type-server-ban",
            NoteType.RoleBan => "ban-guard-note-type-role-ban",
            _ => "ban-guard-note-type-note",
        });
    }

    private static string FlagsText(ServerBanExemptFlags flags)
    {
        return flags == ServerBanExemptFlags.None ? Loc.GetString("ban-guard-none") : flags.ToString();
    }

    private static string TargetsText(IEnumerable<string> names)
    {
        var list = names.ToArray();
        return list.Length == 0 ? Loc.GetString("ban-guard-target-no-user") : string.Join(", ", list);
    }

    private async Task<List<string>> UserLabels(IEnumerable<NetUserId> ids)
    {
        var labels = new List<string>();
        foreach (var id in ids)
        {
            labels.Add(await PlayerLabel(id));
        }

        return labels;
    }

    private async Task<string> AdminLabel(NetUserId? admin)
    {
        if (admin == null)
            return Loc.GetString("system-user");

        return await PlayerLabel(admin.Value);
    }

    private async Task<string> PlayerName(NetUserId id)
    {
        return (await _db.GetPlayerRecordByUserId(id))?.LastSeenUserName ?? id.ToString();
    }

    /// <summary>
    ///     Ник игрока и, если привязан Discord, его упоминание: `Ник` (&lt;@id&gt;).
    /// </summary>
    private async Task<string> PlayerLabel(NetUserId id, string? name = null)
    {
        name ??= await PlayerName(id);
        var label = $"`{name.Replace('`', '\'')}`";

        try
        {
            if (await _db.GetLinkedDiscordId(id, default) is { } discordId)
                label += $" (<@{discordId}>)";
        }
        catch (Exception e)
        {
            // Упоминание - дополнительная информация, без него сообщение всё равно отправляется
            _sawmill.Warning($"Failed to get linked Discord of {id}: {e}");
        }

        return label;
    }
    private static string ExpiryText(DateTime? expiry)
    {
        return expiry is { } time ? DiscordTime(time) : Loc.GetString("ban-guard-never");
    }

    // Unspecified приходит из БД и означает UTC
    private static string DiscordTime(DateTime time)
    {
        var utc = time.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(time, DateTimeKind.Utc)
            : time.ToUniversalTime();

        return $"<t:{new DateTimeOffset(utc).ToUnixTimeSeconds()}:f>";
    }

    #endregion

    #region Sending

    /// <summary>
    ///     Любая ошибка (в том числе при сборке сообщения) только логируется и не должна влиять на баны.
    /// </summary>
    private async Task Guarded(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            _sawmill.Error($"BanGuard failed: {e}");
        }
    }

    private Task Send(BanGuardMessage msg, bool toBansChannel, bool toAdminChannel)
    {
        var tasks = new List<Task>(2);

        _sawmill.Debug($"BanGuard message (bans: {toBansChannel}, admin: {toAdminChannel}):{'\n'}{msg.ToPlainText()}");

        if (toBansChannel)
        {
            tasks.Add(Guarded(() => PostWebhook(
                _cfg.GetCVar(CCVars.DiscordBansWebhook),
                new WebhookPayload { Embeds = [msg.ToWebhookEmbed()] })));
        }

        if (toAdminChannel)
            tasks.Add(Guarded(() => SendToAdminChannel(msg)));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    ///     Бот, если он подключён и канал настроен; иначе вебхук админ-чата, если он задан.
    /// </summary>
    private async Task SendToAdminChannel(BanGuardMessage msg)
    {
        if (_link.IsConnected && GetAdminDiscordChannelId() is ulong channelId && channelId != 0)
        {
            await _link.SendEmbedAsync(channelId, msg.Title, Truncate(msg.Body, EmbedLimit), msg.Color, msg.Footer);
            return;
        }

        await PostWebhook(_cfg.GetCVar(CCVars.DiscordAdminLogsWebhook),
            new WebhookPayload { Embeds = [msg.ToWebhookEmbed()] });
    }
    private async Task PostWebhook(string url, WebhookPayload payload)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        if (await _webhook.GetWebhook(url) is not { } data)
            return;

        await _webhook.CreateMessage(data.ToIdentifier(), payload);
    }

    private static string Truncate(string text, int limit)
    {
        return text.Length <= limit ? text : text[..(limit - 1)] + "…";
    }

    /// <summary>
    ///     Возвращает ID канала для админского Discord. Использует канал логов, если он настроен, иначе возвращает канал админ-чата.
    /// </summary>
    private ulong GetAdminDiscordChannelId()
    {
        if (ulong.TryParse(_cfg.GetCVar(CCVars.AdminLogsDiscordChannelId), out var logsChannelId))
            return logsChannelId;

        if (ulong.TryParse(_cfg.GetCVar(CCVars.AdminChatDiscordChannelId), out var adminChatChannelId))
            return adminChatChannelId;

        return 0;
    }

    #endregion

    private sealed class BanGuardMessage(string title, int color, string footer)
    {
        private readonly List<string> _lines = new();

        public string Title { get; } = title;
        public int Color { get; } = color;
        public string Footer { get; } = footer;
        public string Body => string.Join('\n', _lines);

        public void Add(string locId, params (string, object)[] args)
        {
            _lines.Add(Loc.GetString(locId, args));
        }

        public string ToPlainText() => $"### {Title}\n{Body}\n-# {Footer}";

        public WebhookEmbed ToWebhookEmbed()
        {
            return new WebhookEmbed
            {
                Title = Title,
                Description = Truncate(Body, EmbedLimit),
                Color = Color,
                Footer = new WebhookEmbedFooter { Text = Footer },
            };
        }
    }
}
