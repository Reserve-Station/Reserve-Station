// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Database;
using Content.Shared.Database;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Content.Shared.Administration.Notes;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord.BanGuard;

/// <summary>
///     Reserve Red - BanGuard: отправляет информацию о банах, разбанах и заметках в Discord.
///     Все методы не бросают исключений и не блокируют вызывающий код - ошибки Discord только логируются.
/// </summary>
public interface IBanGuardManager
{
    /// <summary>
    ///     Бан или роль-бан уже записан в БД. Уходит в канал банов и в админский канал.
    /// </summary>
    void BanCreated(BanDef ban);

    /// <summary>
    ///     Бан или роль-бан снят. Уходит в канал банов и в админский канал.
    /// </summary>
    void BanPardoned(BanDef ban, NetUserId? admin);

    /// <summary>
    ///     Заметка добавлена. Уходит только в админский канал.
    /// </summary>
    void NoteAdded(SharedAdminNote note, NetUserId createdBy);

    /// <summary>
    ///     Заметка изменена (в том числе срок действия). Уходит только в админский канал.
    /// </summary>
    void NoteModified(SharedAdminNote oldNote, SharedAdminNote newNote, NetUserId editedBy);

    /// <summary>
    ///     Заметка удалена. Уходит только в админский канал.
    /// </summary>
    void NoteDeleted(SharedAdminNote note, NetUserId deletedBy);

    /// <summary>
    ///     Игрок зашёл на сервер впервые (или недавно). Уходит только в админский канал.
    /// </summary>
    void PlayerFirstJoined(string playerName, NetUserId userId, string creationDate);

    /// <summary>
    ///     Изменены флаги исключения из банов. Уходит только в админский канал.
    /// </summary>
    void BanExemptionChanged(NetUserId player, ServerBanExemptFlags oldFlags, ServerBanExemptFlags newFlags, NetUserId? admin);

    /// <summary>
    ///     Выдан или снят вайтлист на роль. Уходит только в админский канал.
    /// </summary>
    void JobWhitelistChanged(NetUserId player, ProtoId<JobPrototype> job, bool added, NetUserId? admin);
}
