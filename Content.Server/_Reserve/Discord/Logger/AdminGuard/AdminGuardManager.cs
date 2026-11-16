// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server._Reserve.Discord.Logger;
using Content.Server.Database;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord.AdminGuard;

public sealed partial class AdminGuardManager : IAdminGuardManager
{
    [Dependency] private IDiscordLogger _logger = default!;
    [Dependency] private IConsoleHost _console = default!;
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private const int ColorPlaytime = 0x3498db;
    private const int ColorPerms = 0x9b59b6;
    private const int ColorDiscord = 0x1abc9c;
    private const int ColorCurrency = 0xf1c40f;

    private const DiscordLogChannel LogChannel = DiscordLogChannel.AdminLogs;
    private const DiscordLogChannel ChatChannel = DiscordLogChannel.AdminChat;

    public void Initialize()
    {
        _console.AnyCommandExecuted += OnAnyCommandExecuted;
    }

    // Кик через консольную команду движка: на момент вызова цель ещё подключена
    private void OnAnyCommandExecuted(IConsoleShell shell, string commandName, string argStr, string[] args)
    {
        if (commandName != "kick" || args.Length < 1)
            return;

        if (!_players.TryGetSessionByUsername(args[0], out var target))
            return;

        var reason = args.Length >= 2 ? string.Join(' ', args[1..]) : null;
        PlayerKicked(DiscordLogActor.From(target), reason, DiscordLogActor.From(shell.Player));
    }

    #region Админские логи (embed)

    public void PlaytimeChanged(NetUserId target, IReadOnlyCollection<(string Tracker, TimeSpan Time)> changes, bool overwrite, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-playtime", ColorPlaytime);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(target)));

            foreach (var (tracker, time) in changes)
            {
                var minutes = (int) Math.Round(time.TotalMinutes);
                msg.Add(overwrite ? "admin-guard-playtime-set" : "admin-guard-playtime-added",
                    ("tracker", tracker),
                    ("minutes", overwrite ? minutes.ToString() : $"{minutes:+0;-0;0}"));
            }

            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminRankAdded(AdminRankData rank, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-rank-added", ColorPerms);
            AddField(msg, "admin-guard-field-name", rank.Name);
            AddField(msg, "admin-guard-field-flags", FlagsText(rank.Flags, AdminFlags.None));
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminRankUpdated(AdminRankData oldRank, AdminRankData newRank, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-rank-updated", ColorPerms);
            if (oldRank.Name == newRank.Name)
                AddField(msg, "admin-guard-field-name", newRank.Name);
            else
                AddChange(msg, "admin-guard-field-name", oldRank.Name, newRank.Name);
            AddChange(msg, "admin-guard-field-flags", FlagsText(oldRank.Flags, AdminFlags.None), FlagsText(newRank.Flags, AdminFlags.None));
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminRankRemoved(AdminRankData rank, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-rank-removed", ColorPerms);
            AddField(msg, "admin-guard-field-name", rank.Name);
            AddField(msg, "admin-guard-field-flags", FlagsText(rank.Flags, AdminFlags.None));
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminPermsAdded(NetUserId user, AdminPermsData perms, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-perms-added", ColorPerms);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(user)));
            AddPermsFields(msg, perms);
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminPermsUpdated(NetUserId user, AdminPermsData oldPerms, AdminPermsData newPerms, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-perms-updated", ColorPerms);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(user)));
            AddChange(msg, "admin-guard-field-title", TextOrNone(oldPerms.Title), TextOrNone(newPerms.Title));
            AddChange(msg, "admin-guard-field-rank", TextOrNone(oldPerms.Rank), TextOrNone(newPerms.Rank));
            AddChange(msg, "admin-guard-field-flags", FlagsText(oldPerms.Pos, oldPerms.Neg), FlagsText(newPerms.Pos, newPerms.Neg));
            AddChange(msg, "admin-guard-field-suspended", BoolText(oldPerms.Suspended), BoolText(newPerms.Suspended));
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminPermsRemoved(NetUserId user, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-perms-removed", ColorPerms);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(user)));
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void DiscordAccountLinked(NetUserId player, ulong discordId)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-discord-linked", ColorDiscord);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(player)));
            AddField(msg, "admin-guard-field-discord", $"<@{discordId}> (`{discordId}`)");

            await _logger.Send(msg, LogChannel);
        });
    }

    public void DiscordAccountUnlinked(NetUserId? player, ulong discordId)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-discord-unlinked", ColorDiscord);
            msg.Add("admin-guard-target", ("target", player is { } id ? await _logger.PlayerLabel(id) : Loc.GetString("admin-guard-unknown")));
            AddField(msg, "admin-guard-field-discord", $"<@{discordId}> (`{discordId}`)");

            await _logger.Send(msg, LogChannel);
        });
    }

    public void AdminCurrencyChanged(NetUserId target, AdminCurrencyAction action, string amount, string newBalance, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var titleKey = action switch
            {
                AdminCurrencyAction.Add => "admin-guard-title-currency-added",
                AdminCurrencyAction.Remove => "admin-guard-title-currency-removed",
                _ => "admin-guard-title-currency-set",
            };

            var msg = _logger.NewMessage(titleKey, ColorCurrency);
            msg.Add("admin-guard-target", ("target", await _logger.PlayerLabel(target)));

            if (action != AdminCurrencyAction.Set)
                AddField(msg, "admin-guard-field-amount", amount);

            AddField(msg, "admin-guard-field-balance", newBalance);
            msg.Add("admin-guard-by", ("admin", await _logger.ActorLabel(admin)));

            await _logger.Send(msg, LogChannel);
        });
    }

    public void CurrencyTransferred(NetUserId from, NetUserId to, string amount)
    {
        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("admin-guard-title-currency-transfer", ColorCurrency);
            AddField(msg, "admin-guard-field-from", await _logger.PlayerLabel(from));
            AddField(msg, "admin-guard-field-to", await _logger.PlayerLabel(to));
            AddField(msg, "admin-guard-field-amount", amount);

            await _logger.Send(msg, LogChannel);
        });
    }

    #endregion

    #region Админ-чат (обычный текст)

    public void PlayerKicked(DiscordLogActor target, string? reason, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var text = Loc.GetString("admin-guard-kick",
                ("admin", await _logger.ActorLabel(admin)),
                ("target", await _logger.ActorLabel(target)),
                ("reason", string.IsNullOrWhiteSpace(reason) ? Loc.GetString("admin-guard-no-reason") : reason));

            await _logger.SendText(text, ChatChannel);
        });
    }

    public void PlayerFrozen(DiscordLogActor target, AdminFreezeAction action, DiscordLogActor admin)
    {
        _ = _logger.Guarded(async () =>
        {
            var key = action switch
            {
                AdminFreezeAction.Freeze => "admin-guard-freeze",
                AdminFreezeAction.Unfreeze => "admin-guard-unfreeze",
                _ => "admin-guard-freeze-and-mute",
            };

            var text = Loc.GetString(key,
                ("admin", await _logger.ActorLabel(admin)),
                ("target", await _logger.ActorLabel(target)));

            await _logger.SendText(text, ChatChannel);
        });
    }

    public void AdminStatusChanged(AdminStatusChange change, string name, string? title = null)
    {
        _ = _logger.Guarded(async () =>
        {
            if (!string.IsNullOrEmpty(title))
                name += $" \\[{title}\\]";

            var key = change switch
            {
                AdminStatusChange.LoggedIn => "admin-guard-admin-login",
                AdminStatusChange.LoggedOut => "admin-guard-admin-logout",
                AdminStatusChange.Deadmined => "admin-guard-admin-deadmin",
                _ => "admin-guard-admin-readmin",
            };

            await _logger.SendText(Loc.GetString(key, ("name", name)), ChatChannel);
        });
    }

    public void WatchlistPlayerJoined(NetUserId player, AdminWatchlistRecord watchlist)
    {
        // Без бота этим занимается WatchlistWebhookManager, если для него настроен свой вебхук - не дублируем
        if (!_logger.UsesBot(ChatChannel) && !string.IsNullOrWhiteSpace(_cfg.GetCVar(CCVars.DiscordWatchlistConnectionWebhook)))
            return;

        _ = _logger.Guarded(async () =>
        {
            var lines = new List<string>
            {
                Loc.GetString("admin-guard-watchlist-joined",
                    ("player", await _logger.PlayerLabel(player)),
                    ("id", watchlist.Id.ToString())),
                Loc.GetString("admin-guard-watchlist-message", ("message", watchlist.Message)),
                Loc.GetString("admin-guard-watchlist-expiry", ("expiry", watchlist.ExpirationTime is { } expiry
                    ? _logger.DiscordTime(expiry.UtcDateTime)
                    : Loc.GetString("admin-guard-never"))),
            };

            if (watchlist.CreatedBy is { } creator)
                lines.Add(Loc.GetString("admin-guard-by", ("admin", await _logger.PlayerLabel(creator.UserId, creator.LastSeenUserName))));

            await _logger.SendText(string.Join('\n', lines), ChatChannel);
        });
    }

    public void UserIdMismatch(string name, string mainName)
    {
        _ = _logger.Guarded(() => _logger.SendText(
            Loc.GetString("admin-guard-userid-mismatch", ("name", name), ("main", mainName)),
            ChatChannel));
    }

    #endregion

    #region Message building

    private static void AddField(DiscordLogMessage msg, string fieldKey, string value)
    {
        msg.Add("admin-guard-field", ("field", Loc.GetString(fieldKey)), ("value", value));
    }

    private static void AddChange(DiscordLogMessage msg, string fieldKey, string oldValue, string newValue)
    {
        if (oldValue == newValue)
            return;

        msg.Add("admin-guard-field-changed", ("field", Loc.GetString(fieldKey)), ("old", oldValue), ("new", newValue));
    }

    private static void AddPermsFields(DiscordLogMessage msg, AdminPermsData perms)
    {
        AddField(msg, "admin-guard-field-title", TextOrNone(perms.Title));
        AddField(msg, "admin-guard-field-rank", TextOrNone(perms.Rank));
        AddField(msg, "admin-guard-field-flags", FlagsText(perms.Pos, perms.Neg));
        AddField(msg, "admin-guard-field-suspended", BoolText(perms.Suspended));
    }

    private static string FlagsText(AdminFlags pos, AdminFlags neg)
    {
        var text = AdminFlagsHelper.PosNegFlagsText(pos, neg);
        return string.IsNullOrWhiteSpace(text) ? Loc.GetString("admin-guard-none") : text;
    }

    private static string TextOrNone(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? Loc.GetString("admin-guard-none") : text;
    }

    private static string BoolText(bool value)
    {
        return value ? ":white_check_mark:" : ":x:";
    }

    #endregion
}
