// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Content.Server._Reserve.Discord.Logger;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Shared.Administration.Notes;
using Content.Shared.Database;
using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server._Reserve.Discord.BanGuard;

public sealed partial class BanGuardManager : IBanGuardManager, IPostInjectInit
{
    [Dependency] private IDiscordLogger _logger = default!;
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    private const int ColorBan = 0xff0000;
    private const int ColorRoleBan = 0xffa500;
    private const int ColorUnban = 0x2ecc71;
    private const int ColorNote = 0x3498db;
    private const int ColorNoteAdded = 0x3498db;
    private const int ColorNoteModified = 0xf1c40f;
    private const int ColorNoteDeleted = 0x95a5a6;

    private const DiscordLogChannel BanChannels = DiscordLogChannel.Bans | DiscordLogChannel.AdminLogs;
    private const DiscordLogChannel NoteChannels = DiscordLogChannel.AdminLogs;

    private ISawmill _sawmill = default!;

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("discord.ban_guard");
    }

    #region Public API

    public void BanCreated(BanDef ban)
    {
        _ = _logger.Guarded(async () =>
        {
            var isRole = ban.Type == BanType.Role;
            var msg = _logger.NewMessage(
                isRole ? "ban-guard-title-role-ban" : "ban-guard-title-server-ban",
                isRole ? ColorRoleBan : ColorBan,
                ban.Id);

            var names = await _logger.PlayerLabels(ban.UserIds);
            msg.Add("ban-guard-target", ("target", TargetsText(names)));
            msg.Add("ban-guard-reason", ("reason", ban.Reason));
            AddDuration(msg, ban.BanTime, ban.ExpirationTime);

            if (isRole)
            {
                msg.Add("ban-guard-new-roles", ("roles", RolesText(ban.Roles ?? [])));
                foreach (var userId in ban.UserIds)
                {
                    await AddActiveRoles(msg, userId, await _logger.PlayerName(userId));
                }
            }

            msg.Add("ban-guard-banned-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(ban.BanningAdmin))));

            await _logger.Send(msg, BanChannels);
        });
    }

    public void BanPardoned(BanDef ban, NetUserId? admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var isRole = ban.Type == BanType.Role;
            var msg = _logger.NewMessage(
                isRole ? "ban-guard-title-role-unban" : "ban-guard-title-server-unban",
                ColorUnban,
                ban.Id);

            msg.Add("ban-guard-target", ("target", TargetsText(await _logger.PlayerLabels(ban.UserIds))));
            msg.Add("ban-guard-reason", ("reason", ban.Reason));

            if (isRole)
                msg.Add("ban-guard-pardoned-roles", ("roles", RolesText(ban.Roles ?? [])));

            msg.Add("ban-guard-unbanned-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(admin))));

            await _logger.Send(msg, BanChannels);
        });
    }

    public void NoteAdded(SharedAdminNote note, NetUserId createdBy)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("ban-guard-title-note-added", ColorNoteAdded, note.Id, ("type", NoteTypeName(note.NoteType)));
            await AddNoteHeader(msg, note);
            msg.Add("ban-guard-note-message", ("message", note.Message));
            AddNoteDetails(msg, note);
            msg.Add("ban-guard-note-expiry", ("expiry", ExpiryText(note.ExpiryTime)));
            msg.Add("ban-guard-note-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(createdBy))));

            await _logger.Send(msg, NoteChannels);
        });
    }

    public void NoteModified(SharedAdminNote oldNote, SharedAdminNote newNote, NetUserId editedBy)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("ban-guard-title-note-modified", ColorNoteModified, newNote.Id, ("type", NoteTypeName(newNote.NoteType)));
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

            msg.Add("ban-guard-note-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(editedBy))));

            await _logger.Send(msg, NoteChannels);
        });
    }

    public void NoteDeleted(SharedAdminNote note, NetUserId deletedBy)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("ban-guard-title-note-deleted", ColorNoteDeleted, note.Id, ("type", NoteTypeName(note.NoteType)));
            await AddNoteHeader(msg, note);
            msg.Add("ban-guard-note-message", ("message", note.Message));
            AddNoteDetails(msg, note);
            msg.Add("ban-guard-note-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(deletedBy))));

            await _logger.Send(msg, NoteChannels);
        });
    }

    public void PlayerFirstJoined(string playerName, NetUserId userId, string creationDate)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("ban-guard-title-first-join", ColorNote);
            msg.Add("player-first-join-message-webhook", ("name", await _logger.PlayerLabel(userId, playerName)));
            msg.Add("player-first-join-account-date", ("creationDate", creationDate));
            msg.Add("ban-guard-user-id", ("userId", userId.ToString()));

            await _logger.Send(msg, NoteChannels);
        });
    }

    public void BanExemptionChanged(NetUserId player, ServerBanExemptFlags oldFlags, ServerBanExemptFlags newFlags, NetUserId? admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("ban-guard-title-exemption", ColorNote);
            msg.Add("ban-guard-target", ("target", TargetsText(await _logger.PlayerLabels([player]))));
            msg.Add("ban-guard-exemption-changed", ("old", FlagsText(oldFlags)), ("new", FlagsText(newFlags)));
            msg.Add("ban-guard-note-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(admin))));

            await _logger.Send(msg, NoteChannels);
        });
    }

    public void JobWhitelistChanged(NetUserId player, ProtoId<JobPrototype> job, bool added, NetUserId? admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage(added ? "ban-guard-title-job-whitelist-added" : "ban-guard-title-job-whitelist-removed", ColorNote);
            msg.Add("ban-guard-target", ("target", TargetsText(await _logger.PlayerLabels([player]))));
            msg.Add("ban-guard-whitelist-job", ("job", RoleName(new BanRoleDef(BanManager.DbTypeJob, job.Id))));
            msg.Add("ban-guard-note-by", ("admin", await _logger.ActorLabel(new DiscordLogActor(admin))));

            await _logger.Send(msg, NoteChannels);
        });
    }

    #endregion

    #region Message building

    private void AddDuration(DiscordLogMessage msg, DateTimeOffset banTime, DateTimeOffset? expiration)
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
        msg.Add("ban-guard-expires", ("expires", _logger.DiscordTime(expires.UtcDateTime)));
    }

    private async Task AddActiveRoles(DiscordLogMessage msg, NetUserId userId, string userName)
    {
        List<BanDef> bans;
        try
        {
            bans = await _db.GetBansAsync(null, userId, null, null, false, BanType.Role);
        }
        catch (Exception e)
        {
            // Список активных ролей - дополнительная информация, без неё сообщение всё равно отправляется
            _sawmill.Info($"Failed to get active role bans of {userId}: {e}");
            return;
        }

        var roles = bans.SelectMany(b => b.Roles ?? []).Distinct();
        msg.Add("ban-guard-all-roles", ("target", userName), ("roles", RolesText(roles)));
    }

    private async Task AddNoteHeader(DiscordLogMessage msg, SharedAdminNote note)
    {
        msg.Add("ban-guard-target", ("target", TargetsText(await _logger.PlayerLabels(note.Players))));
    }

    private static void AddNoteDetails(DiscordLogMessage msg, SharedAdminNote note)
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

    private string ExpiryText(DateTime? expiry)
    {
        return expiry is { } time ? _logger.DiscordTime(time) : Loc.GetString("ban-guard-never");
    }

    #endregion
}
