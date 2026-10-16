// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord.Logger;

/// <summary>
///     Единая точка отправки логов в Discord (бот или вебхуки).
///     Ничего не бросает наружу: неверная настройка или ошибка Discord только логируются.
/// </summary>
public interface IDiscordLogger
{
    /// <summary>
    ///     Выполняет действие, перехватывая и логируя любые исключения.
    ///     Использовать для всей сборки и отправки сообщения: <c>_ = _logger.Guarded(async () => ...)</c>.
    /// </summary>
    Task Guarded(Func<Task> action);

    /// <summary>
    ///     Будет ли сообщение в этот канал отправлено через бота (а не вебхук).
    /// </summary>
    bool UsesBot(DiscordLogChannel channel);

    /// <summary>
    ///     Создаёт сообщение с локализованным заголовком (аргумент <c>$id</c> + <paramref name="titleArgs"/>)
    ///     и подвалом "сервер (раунд)".
    /// </summary>
    DiscordLogMessage NewMessage(string titleKey, int color, int? id = null, params (string, object)[] titleArgs);

    /// <summary>
    ///     Отправляет сообщение как embed во все выбранные каналы.
    /// </summary>
    Task Send(DiscordLogMessage message, DiscordLogChannel channels);

    /// <summary>
    ///     Отправляет обычный текст (без embed) во все выбранные каналы.
    /// </summary>
    Task SendText(string text, DiscordLogChannel channels);

    /// <summary>
    ///     Ник игрока и, если привязан Discord, его упоминание.
    /// </summary>
    Task<string> PlayerLabel(NetUserId id, string? name = null);

    Task<List<string>> PlayerLabels(IEnumerable<NetUserId> ids);

    Task<string> ActorLabel(DiscordLogActor actor);

    /// <summary>
    ///     Последний известный ник игрока из БД или его ID.
    /// </summary>
    Task<string> PlayerName(NetUserId id);

    /// <summary>
    ///     Время в формате Discord-таймстемпа.
    /// </summary>
    string DiscordTime(DateTime time);
}
