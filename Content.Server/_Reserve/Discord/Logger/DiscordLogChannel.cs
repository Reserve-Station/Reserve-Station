// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Reserve.Discord.Logger;

/// <summary>
///     Куда отправлять лог-сообщение. Каналы можно комбинировать.
/// </summary>
[Flags]
public enum DiscordLogChannel
{
    None = 0,

    /// <summary>
    ///     Только вебхук банов (<c>discord.bans_webhook</c>).
    /// </summary>
    Bans = 1 << 0,

    /// <summary>
    ///     Админские логи: бот (канал логов, иначе канал админ-чата), иначе вебхук логов, иначе вебхук админ-чата.
    /// </summary>
    AdminLogs = 1 << 1,

    /// <summary>
    ///     Админ-чат: бот (канал админ-чата), иначе вебхук админ-чата.
    /// </summary>
    AdminChat = 1 << 2,
}
