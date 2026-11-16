// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Reserve.Discord.Logger;
using Content.Server.Database;
using Content.Shared.Administration;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord.AdminGuard;

public enum AdminCurrencyAction
{
    Add,
    Remove,
    Set,
}

public enum AdminFreezeAction
{
    Freeze,
    Unfreeze,
    FreezeAndMute,
}

public enum AdminStatusChange
{
    LoggedIn,
    LoggedOut,
    Deadmined,
    Readmined,
}

/// <summary>
///     Название и флаги группы прав.
/// </summary>
public readonly record struct AdminRankData(string Name, AdminFlags Flags);

/// <summary>
///     Права конкретного админа: титул, группа, свои флаги и приостановка.
/// </summary>
public readonly record struct AdminPermsData(string? Title, string? Rank, AdminFlags Pos, AdminFlags Neg, bool Suspended);

/// <summary>
///     Reserve Red - AdminGuard: отправляет в Discord действия, полезные админам.
///     Разовые действия (права, наигранное время, монеты, привязки) уходят embed'ом в админские логи,
///     обычные события (кик, заморозка, вход/выход админов и т.п.) - обычным текстом в админ-чат.
///     Все методы не бросают исключений и не блокируют вызывающий код.
/// </summary>
public interface IAdminGuardManager
{
    void Initialize();

    #region Админские логи (embed)

    /// <summary>
    ///     Изменено наигранное время игрока: добавлено (<paramref name="overwrite"/> = false) или перезаписано.
    /// </summary>
    void PlaytimeChanged(NetUserId target, IReadOnlyCollection<(string Tracker, TimeSpan Time)> changes, bool overwrite, DiscordLogActor admin);

    void AdminRankAdded(AdminRankData rank, DiscordLogActor admin);

    void AdminRankUpdated(AdminRankData oldRank, AdminRankData newRank, DiscordLogActor admin);

    void AdminRankRemoved(AdminRankData rank, DiscordLogActor admin);

    void AdminPermsAdded(NetUserId user, AdminPermsData perms, DiscordLogActor admin);

    void AdminPermsUpdated(NetUserId user, AdminPermsData oldPerms, AdminPermsData newPerms, DiscordLogActor admin);

    void AdminPermsRemoved(NetUserId user, DiscordLogActor admin);

    void DiscordAccountLinked(NetUserId player, ulong discordId);

    /// <summary>
    ///     <paramref name="player"/> может быть null, если аккаунт уже не привязан к игроку.
    /// </summary>
    void DiscordAccountUnlinked(NetUserId? player, ulong discordId);

    /// <summary>
    ///     Админ изменил баланс игрока. Суммы уже отформатированы с названием валюты.
    /// </summary>
    void AdminCurrencyChanged(NetUserId target, AdminCurrencyAction action, string amount, string newBalance, DiscordLogActor admin);

    /// <summary>
    ///     Игрок перевёл монеты другому игроку.
    /// </summary>
    void CurrencyTransferred(NetUserId from, NetUserId to, string amount);

    #endregion

    #region Админ-чат (обычный текст)

    void PlayerKicked(DiscordLogActor target, string? reason, DiscordLogActor admin);

    void PlayerFrozen(DiscordLogActor target, AdminFreezeAction action, DiscordLogActor admin);

    void AdminStatusChanged(AdminStatusChange change, string name, string? title = null);

    /// <summary>
    ///     Зашёл игрок с активным наблюдением (watchlist).
    /// </summary>
    void WatchlistPlayerJoined(NetUserId player, AdminWatchlistRecord watchlist);

    /// <summary>
    ///     Игрок пытается подключиться с UserID другого аккаунта.
    /// </summary>
    void UserIdMismatch(string name, string mainName);

    #endregion
}
